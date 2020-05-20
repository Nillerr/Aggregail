import React, {useEffect, useState} from "react";
import {RecordedEvent} from "./StreamHub";
import {Redirect} from "react-router";
import {Link} from "react-router-dom";

const dummyEvent: RecordedEvent = {
  id: "0b34563453",
  stream: "Case-a5b53551-d608-45b0-9845-98cdc7c23454",
  eventId: "4f51fc99-cc60-47ec-a9a7-5207f167b2ad",
  eventType: "CaseImported",
  eventNumber: 1,
  created: '2020-05-20T07:29:02Z',
  data: {
    title: "The Title",
    subject: "The Subject",
    caseNumber: "TS012345",
  }
};

type EventState = { kind: 'Loading' } | { kind: 'Loaded', event: RecordedEvent | null };

const Event = (props: { stream: string, eventNumber: number }) => {
  
  const [eventState, setEventState] = useState<EventState>({ kind: 'Loading' });
  
  useEffect(() => {
    setTimeout(() => setEventState({ kind: 'Loaded', event: dummyEvent }), 1_000);
  });
  
  if (eventState.kind === 'Loading') {
    return (<div>Loading...</div>)
  }
  
  const event = eventState.event;
  if (event === null) {
    return (<Redirect to={`/streams/${props.stream}`}/>);
  }
  
  return (
    <React.Fragment>
      <h5 style={{marginBottom: '20px'}}>{event.eventNumber}@{event.stream}</h5>
      <div className="row">
        <table className="table table-bordered">
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
            <td scope="row">{event.eventNumber}</td>
            <td><Link to={`/streams/${event.stream}`}>{event.stream}</Link></td>
            <td>{event.eventType}</td>
            <td>{new Date(event.created).toLocaleString()}</td>
          </tr>
          <tr>
            <td colSpan={4}>
              <strong>Data</strong>
              <pre>{JSON.stringify(event.data, null, 2)}</pre>
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
            <td colSpan={3}>{event.eventId}</td>
          </tr>
          </tbody>
        </table>
      </div>
    </React.Fragment>
  );
}

export default Event;