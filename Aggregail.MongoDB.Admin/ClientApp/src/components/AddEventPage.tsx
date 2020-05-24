import {useAction, useRequest} from "../hooks";
import {Alert, Button, Form, Input, Spinner, Table} from "reactstrap";
import React, {useState} from "react";
import {RecordedEvent} from "./StreamHub";
import * as uuid from 'uuid';
import JsonEditor from "./JsonEditor";

interface Event {
  stream: string;
  eventId: string;
  eventType: string;
  data: string;
}

const EventEditor = (props: { event: Event, onEventChange: (event: Event) => void }) => {
  return (
    <Table responsive bordered size="sm" className="mt-2 mb-2">
      <tbody>
      <tr>
        <td className="align-middle">Stream</td>
        <td>
          <Input
            type="text"
            value={props.event.stream}
            onChange={e => props.onEventChange({ ...props.event, stream: e.target.value })}
          />
        </td>
      </tr>
      <tr>
        <td className="align-middle">Event ID</td>
        <td>
          <Input
            type="text"
            value={props.event.eventId}
            onChange={e => props.onEventChange({ ...props.event, eventId: e.target.value })}
          />
        </td>
      </tr>
      <tr>
        <td className="align-middle">Event Type</td>
        <td>
          <Input
            type="text"
            value={props.event.eventType}
            onChange={e => props.onEventChange({ ...props.event, eventType: e.target.value })}
          />
        </td>
      </tr>
      </tbody>
      <tbody className="thead-dark">
      <tr>
        <th colSpan={2}>Data</th>
      </tr>
      </tbody>
      <tbody>
      <tr>
        <td colSpan={2}>
          <JsonEditor
            value={props.event.data}
            onValueChange={value => props.onEventChange({ ...props.event, data: value })}
          />
        </td>
      </tr>
      </tbody>
    </Table>
  );
}

interface AppendEventRequest {
  eventId: string;
  eventType: string;
  data: any;
}

const AddEvent = (props: { stream?: string, likeEvent?: RecordedEvent, onEventAdded: (stream: string) => void }) => {

  const [event, setEvent] = useState<Event>(() => {
    const clonedEventProps = props.likeEvent
      ? ({
        eventType: props.likeEvent.eventType,
        data: JSON.stringify(props.likeEvent.data, null, 2)
      })
      : {eventType: '', data: '{}'};

    return {
      stream: props.stream ?? '',
      eventId: uuid.v4(),
      ...clonedEventProps
    };
  });
  
  const submitAction = useAction<AppendEventRequest>('POST', `/api/streams/${event.stream}`, () => props.onEventAdded(event.stream));
  
  const submit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    submitAction({
      eventId: event.eventId,
      eventType: event.eventType,
      data: JSON.parse(event.data)
    });
  };

  return (
    <Form onSubmit={submit}>
      <EventEditor event={event} onEventChange={setEvent}/>
      <Button type="submit" color="primary">Add Event</Button>
    </Form>
  );
}

const AddEventPage = (props: { stream?: string, onEventAdded: (stream: string) => void }) => {
  return (
    <React.Fragment>
      <div className="mt-2 mb-2">
        <h5 className="mb-3 mr-auto">New Event</h5>
      </div>
      <AddEvent stream={props.stream} onEventAdded={props.onEventAdded}/>
    </React.Fragment>
  )
};

const AddEventLikePage = (props: { stream: string, eventNumber: number, onEventAdded: (stream: string) => void }) => {
  const likeEventState = useRequest<RecordedEvent>(`/api/streams/${props.stream}/${props.eventNumber}`);

  return (
    <React.Fragment>
      <div className="mt-2 mb-2">
        <h5 className="mb-3 mr-auto">New Event</h5>
      </div>
      {likeEventState.kind === 'Loading'
        ? <div className="text-center"><Spinner>Loading...</Spinner></div>
        : null
      }
      {likeEventState.kind === 'Failed'
        ? <Alert color="danger">{`${likeEventState.reason}`}</Alert>
        : null
      }
      {likeEventState.kind === 'Loaded'
        ? <AddEvent stream={props.stream} likeEvent={likeEventState.data} onEventAdded={props.onEventAdded}/>
        : null
      }
    </React.Fragment>
  )
};

export {AddEventPage, AddEventLikePage};
