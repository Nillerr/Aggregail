import React from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";
import {Button, Table} from "reactstrap";
import {StreamResponse} from "../model";
import {usePauseablePolling} from "../hooks";
import {LoadableTableBody} from "./LoadableTableBody";

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

interface StreamPageProps {
  name: string;
  from: number;
  limit: number;
  onAddEvent: (stream: string) => void;
  onDelete: (stream: string) => void;
  onReset: () => void;
  onFirst: () => void;
  onNext: () => void;
  onPrevious: () => void;
}

const StreamPage = (props: StreamPageProps) => {

  const [streamState, isPaused, setIsPaused] = usePauseablePolling<StreamResponse>(
    `/api/streams/${props.name}/${props.from}/forward/${props.limit}`
  );

  const events = streamState.kind === 'Loaded'
    ? streamState.data.events
    : [];

  const latestEvent = max(events.map(e => e.eventNumber));

  return (
    <React.Fragment>
      <div className="d-flex mt-2 mb-2 flex-wrap">
        <h5 className="mr-auto">Event Stream '{props.name}'</h5>
        <div className="btn-group flex-grow-1 flex-sm-grow-0" role="group">
          <Button color="secondary" onClick={() => setIsPaused(!isPaused)}>
            {isPaused ? 'Resume' : 'Pause'}
          </Button>
          <Button color="secondary" onClick={() => props.onDelete(props.name)}>Delete</Button>
          <Button color="secondary" onClick={() => props.onAddEvent(props.name)}>Add Event</Button>
          <Button tag={Link} color="secondary" to={'/streams'}>Back</Button>
        </div>
      </div>
      <div className="d-flex mt-2 mb-2">
        <div className="btn-group flex-grow-1 flex-sm-grow-0" role="group">
          <Button
            color="secondary"
            onClick={() => props.onReset()}
          >Reset</Button>
          <Button
            color="secondary"
            disabled={props.from === 0}
            onClick={() => props.onFirst()}
          >First</Button>
          <Button
            color="secondary"
            disabled={props.from === 0}
            onClick={() => props.onPrevious()}
          >Previous</Button>
          <Button
            color="secondary"
            disabled={latestEvent === undefined || events.length < props.limit}
            onClick={() => props.onNext()}
          >Next</Button>
        </div>
      </div>
      <Table responsive bordered size="sm">
        <thead className="thead-dark">
        <tr>
          <th scope="col">Event #</th>
          <th scope="col">Name</th>
          <th scope="col">Type</th>
          <th scope="col">Created Date</th>
          <th scope="col">&nbsp;</th>
        </tr>
        </thead>
        <LoadableTableBody
          colSpan={5}
          state={streamState}
          render={({data: {events}}) => events.map(event => <EventRow key={event.eventId} event={event}/>)}
        />
      </Table>
    </React.Fragment>
  );
};

export default StreamPage;