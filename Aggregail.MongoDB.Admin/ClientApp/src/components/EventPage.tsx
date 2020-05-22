import React, {useEffect, useState} from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";
import {Alert, Button, ButtonGroup, Spinner, Table} from 'reactstrap';
import Axios from "axios";
import Json from "./Json";

type EventState =
  | { kind: 'Loading' }
  | { kind: 'Loaded', event: RecordedEvent }
  | { kind: 'Failed', reason: any };

const loadEventState = (stream: string, eventNumber: number, setState: (state: EventState) => void) => {
  const cts = Axios.CancelToken.source();

  Axios
    .get<RecordedEvent>(`/api/streams/${stream}/${eventNumber}`, {cancelToken: cts.token})
    .then(response => setState({kind: 'Loaded', event: response.data}))
    .catch(reason => setState({kind: 'Failed', reason: reason}));

  return cts;
}

const EventButton = (props: { label: string, event?: { stream: string, eventNumber: number } }) => {
  return props.event
    ? (
      <Button
        tag={Link}
        color="secondary"
        to={`/streams/${props.event.stream}/${props.event.eventNumber}`}
      >{props.label}</Button>
    )
    : (<Button type="button" color="secondary" disabled={true}>{props.label}</Button>);
};

const EventContent = (props: { event: RecordedEvent }) => {
  return (
    <React.Fragment>
      <Table responsive bordered size="sm" className="mt-2 mb-2">
        <thead className="thead-dark">
        <tr>
          <th scope="col">No</th>
          <th scope="col">Stream</th>
          <th scope="col">Type</th>
          <th scope="col">Timestamp</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>{props.event.eventNumber}</td>
          <td><Link to={`/streams/${props.event.stream}`}>{props.event.stream}</Link></td>
          <td>{props.event.eventType}</td>
          <td>{new Date(props.event.created).toLocaleString()}</td>
        </tr>
        <tr>
          <td colSpan={4}>
            <strong>Data</strong>
            <div className="mt-2 mb-4">
              <Json>
                {JSON.stringify(props.event.data, null, 2)}
              </Json>
            </div>
          </td>
        </tr>
        </tbody>
        <thead className="thead-dark">
        <tr>
          <th scope="col" colSpan={4}>Internal data</th>
        </tr>
        </thead>
        <tbody>
        <tr>
          <td>EventId</td>
          <td colSpan={3}>{props.event.eventId}</td>
        </tr>
        </tbody>
      </Table>
    </React.Fragment>
  );
}

const EventPage = (props: { stream: string, eventNumber: number }) => {

  const [eventState, setEventState] = useState<EventState>({kind: 'Loading'});
  const [nextEventState, setNextEventState] = useState<EventState>({kind: 'Loading'});

  useEffect(() => {
    const loadEventToken = loadEventState(props.stream, props.eventNumber, setEventState);
    const loadNextEventToken = loadEventState(props.stream, props.eventNumber + 1, setNextEventState);

    return () => {
      loadEventToken.cancel();
      loadNextEventToken.cancel();
    };
  }, [props.stream, props.eventNumber]);

  const previousEvent = props.eventNumber === 0
    ? undefined
    : {stream: props.stream, eventNumber: props.eventNumber - 1};

  const nextEvent = nextEventState.kind === 'Loaded'
    ? nextEventState.event
    : undefined;

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2 flex-wrap">
        <h5 className="mb-3 mr-auto">{props.eventNumber}@{props.stream}</h5>
        <ButtonGroup className="flex-grow-1 flex-sm-grow-0" role="group">
          <Button type="button" color="secondary" disabled={true}>Add New Like This</Button>
          <Button tag={Link} color="secondary" to={`/streams/${props.stream}`}>Back</Button>
        </ButtonGroup>
      </div>
      <div className="d-flex mt-2 mb-2">
        <div className="btn-group flex-grow-1 flex-sm-grow-0" role="group">
          <EventButton label="Previous" event={previousEvent}/>
          <EventButton label="Next" event={nextEvent}/>
        </div>
      </div>
      {eventState.kind === 'Loading'
        ? <div className="d-flex justify-content-center">
          <Spinner>Loading...</Spinner>
        </div>
        : null
      }
      {eventState.kind === 'Failed'
        ? <Alert color="danger">{eventState.reason}</Alert>
        : null
      }
      {eventState.kind === 'Loaded'
        ? <EventContent event={eventState.event}/>
        : null
      }
    </React.Fragment>
  );
}

export default EventPage;