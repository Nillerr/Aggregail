import React, {useEffect, useState} from "react";
import {Link} from "react-router-dom";
import Axios, {CancelToken, CancelTokenStatic} from "axios";
import {Alert, Button, Form, Input, InputGroup, InputGroupAddon, Spinner, Table} from "reactstrap";

const StreamRow = (props: { name: string }) => (
  <tr>
    <td><Link to={`/streams/${props.name}`}>{props.name}</Link></td>
  </tr>
);

const StreamsTable = (props: { legend: string, state: StreamsTableState }) => (
  <Table bordered={true} size="sm">
    <thead className="thead-dark">
    <tr>
      <th scope="col">{props.legend}</th>
    </tr>
    </thead>
    <tbody>
    {props.state.kind === 'Loading'
      ? <tr>
        <td className="text-center"><Spinner>Loading...</Spinner></td>
      </tr>
      : null
    }
    {props.state.kind === 'Failed'
      ? <tr>
        <td><Alert color="danger">{`${props.state.reason}`}</Alert></td>
      </tr>
      : null
    }
    {props.state.kind === 'Loaded'
      ? props.state.streams.length > 0
        ? props.state.streams.map(stream => <StreamRow key={stream.name} name={stream.name}/>)
        : <tr><td>No streams</td></tr>
      : null
    }
    </tbody>
  </Table>
);

interface RecentStream {
  name: string;
}

interface RecentStreams {
  recentlyCreatedStreams: RecentStream[];
  recentlyChangedStreams: RecentStream[];
}

type StreamsTableState =
  | { kind: 'Loading' }
  | { kind: 'Loaded', streams: RecentStream[] }
  | { kind: 'Failed', reason: any };

const StreamBrowserPage = (props: { onStreamBrowse: (stream: string) => void }) => {

  const [recentlyCreatedStreams, setRecentlyCreatedStreams] = useState<StreamsTableState>({kind: 'Loading'});
  const [recentlyChangedStreams, setRecentlyChangedStreams] = useState<StreamsTableState>({kind: 'Loading'});
  const [currentReloadTimeout, setCurrentReloadTimeout] = useState<NodeJS.Timeout>();
  
  const loadStreams = (cancelToken: CancelToken) => {
    return Axios
      .get<RecentStreams>('/api/streams', {cancelToken})
      .then(response => {
        setRecentlyCreatedStreams({kind: 'Loaded', streams: response.data.recentlyCreatedStreams});
        setRecentlyChangedStreams({kind: 'Loaded', streams: response.data.recentlyChangedStreams});
      })
      .catch(reason => {
        setRecentlyCreatedStreams({kind: "Failed", reason});
        setRecentlyChangedStreams({kind: "Failed", reason});
      });
  };
  
  const reloadStreams = (cancelToken: CancelToken) => {
    const timeout = setTimeout(
      () => loadStreams(cancelToken).finally(() => reloadStreams(cancelToken)),
      2_500
    );
    
    setCurrentReloadTimeout(timeout);
  }
  
  useEffect(() => {
    const cts = Axios.CancelToken.source();

    loadStreams(cts.token)
      .finally(() => reloadStreams(cts.token));

    return () => {
      cts.cancel();
      
      if (currentReloadTimeout) {
        clearTimeout(currentReloadTimeout);
      }
    };
  }, []);

  const [stream, setStream] = useState('');

  return (
    <React.Fragment>
      <div className="d-flex mb-4">
        <h5 className="mr-auto">Stream Browser</h5>
        <div>
          <Form onSubmit={() => props.onStreamBrowse(stream)}>
            <InputGroup>
              <Input className="" type="text" placeholder="Stream" value={stream}
                     onChange={e => setStream(e.target.value)}/>
              <InputGroupAddon addonType="append" className="mr-2">
                <Button type="submit" outline={true} color="secondary" className="text-nowrap">Browse</Button>
              </InputGroupAddon>
            </InputGroup>
          </Form>
        </div>
        <div>
          <Button className="ml-2 flex-shrink-0 text-nowrap" outline={true} color="secondary" disabled={true}>
            Add Event
          </Button>
        </div>
      </div>
      <div className="row">
        <div className="col-12 col-md-6">
          <StreamsTable
            legend="Recently Created Streams"
            state={recentlyCreatedStreams}
          />
        </div>
        <div className="col-12 col-md-6">
          <StreamsTable
            legend="Recently Changed Streams"
            state={recentlyChangedStreams}
          />
        </div>
      </div>
    </React.Fragment>
  );

}

export default StreamBrowserPage;