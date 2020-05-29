import React from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";
import {Button, ButtonGroup, Table} from 'reactstrap';
import Json from "./Json";
import {useRequest} from "../hooks";
import {LoadableTableBody} from "./LoadableTableBody";

const useEvent = (stream: string, eventNumber: number) => {
  return useRequest<RecordedEvent>(`/api/streams/${stream}/${eventNumber}`);
};

const EventButton = (props: { label: string, event?: { stream: string, eventNumber: number } }) => {
  return props.event
    ? (
      <Button
        tag={Link}
        color="secondary"
        to={`/streams/${props.event.stream}/${props.event.eventNumber}`}
      >{props.label}</Button>
    )
    : (
      <Button
        type="button"
        color="secondary"
        disabled={true}
      >{props.label}</Button>
    );
};

const EventContent = (props: { event: RecordedEvent }) => (
  <React.Fragment>
    <tr>
      <td>{props.event.eventNumber}</td>
      <td><Link to={`/streams/${props.event.stream}`}>{props.event.stream}</Link></td>
      <td>{props.event.eventType}</td>
      <td>{new Date(props.event.created).toLocaleString()}</td>
    </tr>
    <tr>
      <td>EventId</td>
      <td colSpan={3}>{props.event.eventId}</td>
    </tr>
    <tr>
      <td colSpan={4}>
        <strong>Data</strong>
        <div className="mt-2">
          <Json>
            {JSON.stringify(props.event.data, null, 2)}
          </Json>
        </div>
      </td>
    </tr>
    <tr>
      <td colSpan={4}>
        <strong>Metadata</strong>
        <div className="mt-2">
          <Json>
            {JSON.stringify(props.event.metadata, null, 2)}
          </Json>
        </div>
      </td>
    </tr>
  </React.Fragment>
);

const EventPage = (props: { stream: string, eventNumber: number, onAddEventLike: (stream: string, eventNumber: number) => void }) => {

  const eventState = useEvent(props.stream, props.eventNumber);
  const nextEventState = useEvent(props.stream, props.eventNumber + 1);

  const previousEvent = props.eventNumber === 0
    ? undefined
    : {stream: props.stream, eventNumber: props.eventNumber - 1};

  const nextEvent = nextEventState.kind === 'Loaded'
    ? nextEventState.data
    : undefined;

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2 flex-wrap">
        <h5 className="mb-3 mr-auto">{props.eventNumber}@{props.stream}</h5>
        <ButtonGroup className="flex-grow-1 flex-sm-grow-0" role="group">
          <Button type="button" color="secondary" onClick={() => props.onAddEventLike(props.stream, props.eventNumber)}>Add New Like This</Button>
          <Button tag={Link} color="secondary" to={`/streams/${props.stream}`}>Back</Button>
        </ButtonGroup>
      </div>
      <div className="d-flex mt-2 mb-2">
        <div className="btn-group flex-grow-1 flex-sm-grow-0" role="group">
          <EventButton label="Previous" event={previousEvent}/>
          <EventButton label="Next" event={nextEvent}/>
        </div>
      </div>
      <Table responsive bordered size="sm" className="mt-2 mb-2">
        <thead className="thead-dark">
        <tr>
          <th scope="col">No</th>
          <th scope="col">Stream</th>
          <th scope="col">Type</th>
          <th scope="col">Timestamp</th>
        </tr>
        </thead>
        <LoadableTableBody
          colSpan={4}
          state={eventState}
          render={state => <EventContent event={state.data}/>}
        />
      </Table>
    </React.Fragment>
  );
};

export default EventPage;