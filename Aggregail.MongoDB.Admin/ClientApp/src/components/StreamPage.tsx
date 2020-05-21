import React, {useState} from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";
import {Alert, Button, Spinner, Table} from "reactstrap";
import {StreamResponse} from "../model";
import {usePolling} from "../hooks";
import {LoadableState} from "../lib";

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

const max = (source: number[]): number | undefined => {
  let max: number | undefined = undefined;
  for (const s of source) {
    if (max === undefined || max < s) {
      max = s;
    }
  }
  return max;
}

const StreamPage = (props: { from: number, direction: 'backward' | 'forward', name: string, onNext: (from: number) => void, onPrevious: (from: number) => void }) => {

  const limit = 20;
  const streamState = usePolling<StreamResponse>(`/api/streams/${props.name}/${props.from}/${props.direction}/${limit}`);
  const stream = LoadableState.map(streamState, data => ({events: data.events}));

  const events = stream.kind === 'Loaded'
    ? stream.events
    : [];

  const latestEvent = max(events.map(e => e.eventNumber));

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2">
        <h5 className="mr-auto">Event Stream '{props.name}'</h5>
        <div className="btn-group" role="group">
          <Button color="secondary" outline={true} disabled={true}>Pause</Button>
          <Button color="secondary" outline={true} disabled={true}>Delete</Button>
          <Button color="secondary" outline={true} disabled={true}>Add Event</Button>
          <Button tag={Link} color="secondary" outline={true} to={'/streams'}>Back</Button>
        </div>
      </div>
      <div className="mt-2 mb-2">
        <div className="btn-group" role="group">
          <Button color="secondary" outline={true} disabled={true}>Self</Button>
          <Button color="secondary" outline={true} disabled={true}>First</Button>
          <Button
            color="secondary" 
            outline={true}
            disabled={latestEvent === undefined || props.from === 0}
            onClick={() => props.onPrevious(0)}>Previous</Button>
          <Button
            color="secondary"
            outline={true}
            disabled={latestEvent === undefined || events.length < limit}
            onClick={() => props.onNext(limit)}
          >Next</Button>
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