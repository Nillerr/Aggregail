import React, {useEffect, useState} from "react";
import {RecordedEvent} from "./StreamHub";
import {Link} from "react-router-dom";

const dummyEvents: RecordedEvent[] = [
  {
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
  },
  {
    id: "0b123456",
    stream: "Case-a5b53551-d608-45b0-9845-98cdc7c23454",
    eventId: "a5b53551-d608-45b0-9845-98cdc7c23454",
    eventType: "CaseCreated",
    eventNumber: 0,
    created: '2020-05-20T07:29:02Z',
    data: {
      title: "The Title",
      subject: "The Subject",
    }
  }
]; 

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

const Stream = (props: { name: string }) => {
  
  const [page, setPage] = useState(1);
  const [numberOfPages, setNumberOfPages] = useState(0);
  const [events, setEvents] = useState<RecordedEvent[]>([]);
  
  useEffect(() => {
    setTimeout(() => setEvents(dummyEvents), 1_000);
  })
  
  return (
    <div className="row">
      <div className="col-12">
        <table className="table table-bordered">
          <thead className="thead-dark">
          <tr>
            <th>Event #</th>
            <th>Name</th>
            <th>Type</th>
            <th>Created Date</th>
            <th>&nbsp;</th>
          </tr>
          </thead>
          <tbody>
          {events.map(event => (<EventRow key={event.eventId} event={event}/>))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default Stream;