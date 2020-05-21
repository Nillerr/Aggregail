import React, {useState} from "react";
import {Link} from "react-router-dom";
import {Alert, Button, Form, Input, InputGroup, InputGroupAddon, Spinner, Table} from "reactstrap";
import {usePolling} from "../hooks";
import {LoadableState} from "../lib";
import {RecentStream, RecentStreams} from "../model";

const StreamRow = (props: { name: string }) => (
  <tr>
    <td><Link to={`/streams/${props.name}`}>{props.name}</Link></td>
  </tr>
);

const StreamsTable = (props: { legend: string, state: LoadableState<{ streams: RecentStream[] }> }) => (
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

const StreamBrowserPage = (props: { onStreamBrowse: (stream: string) => void }) => {

  const recentStreams = usePolling<RecentStreams>('/api/streams');
  
  const recentlyCreatedStreams = LoadableState.map(recentStreams, data => ({ streams: data.recentlyCreatedStreams }));
  const recentlyChangedStreams = LoadableState.map(recentStreams, data => ({ streams: data.recentlyChangedStreams }));

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