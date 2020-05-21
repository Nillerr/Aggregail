import React, {useEffect, useState} from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";
import Axios from "axios";
import {Alert, Spinner, Table} from "reactstrap";

const EventRow = (props: { event: RecordedEvent }) => {
  let name = `${props.event.eventNumber}@${props.event.stream}`;
  let destination = `/streams/${props.event.stream}/${props.event.eventNumber}`;
  return (
    <tr>
      <td><Link to={destination}>{props.event.eventNumber}</Link></td>
      <td><Link to={destination}>{name}</Link></td>
      <td>{props.event.eventType}</td>
      <td>{new Date(props.event.created).toLocaleString()}</td>
      <td><Link to={destination}>JSON</Link></td>
    </tr>
  );
};

interface StreamResponse {
  events: RecordedEvent[];
}

type StreamPageState =
  | { kind: 'Loading' }
  | { kind: 'Loaded' } & StreamResponse
  | { kind: 'Failed', reason: any };

const StreamPage = (props: { name: string }) => {

  // const [page, setPage] = useState(1);
  // const [numberOfPages, setNumberOfPages] = useState(0);

  const [stream, setStream] = useState<StreamPageState>({kind: 'Loading'});

  useEffect(() => {
    const cts = Axios.CancelToken.source();

    Axios
      .get<StreamResponse>(`/api/streams/${props.name}`, {cancelToken: cts.token})
      .then(response => setStream({kind: 'Loaded', ...response.data}))
      .catch(reason => setStream({kind: 'Failed', reason}));

    return () => {
      cts.cancel();
    };
  }, [props.name])

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2">
        <h5 className="mr-auto">Event Stream '{props.name}'</h5>
        <div className="btn-group" role="group">
          <button className="btn btn-outline-secondary" disabled={true}>Pause</button>
          <button className="btn btn-outline-secondary" disabled={true}>Delete</button>
          <button className="btn btn-outline-secondary" disabled={true}>Add Event</button>
          <Link className="btn btn-outline-secondary" to={'/streams'}>Back</Link>
        </div>
      </div>
      <div className="mt-2 mb-2">
        <div className="btn-group" role="group">
          <button className="btn btn-outline-secondary" disabled={true}>Self</button>
          <button className="btn btn-outline-secondary" disabled={true}>First</button>
          <button className="btn btn-outline-secondary" disabled={true}>Previous</button>
        </div>
      </div>
      <Table bordered={true} size="sm">
        <thead className="thead-dark">
        <tr>
          <th scope="col">Event #</th>
          <th scope="col">Name</th>
          <th scope="col">Type</th>
          <th scope="col">Created Date</th>
          <th scope="col">&nbsp;</th>
        </tr>
        </thead>
        <tbody>
        {stream.kind === 'Loaded'
          ? stream.events.map(event => (<EventRow key={event.eventId} event={event}/>))
          : null
        }
        </tbody>
      </Table>
      {stream.kind === 'Loading'
        ? <div className="d-flex justify-content-center">
          <Spinner>Loading...</Spinner>
        </div>
        : null
      }
      {stream.kind === 'Failed'
        ? <Alert color="danger">{`${stream.reason}`}</Alert>
        : null
      }
    </React.Fragment>
  );
};

export default StreamPage;