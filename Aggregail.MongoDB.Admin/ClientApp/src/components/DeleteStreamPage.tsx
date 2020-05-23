import React from "react";
import {Alert, Button} from "reactstrap";
import {useAction} from "../hooks";

const DeleteStreamPage = (props: ({ stream: string, onCancel: (stream: string) => void, onDelete: (stream: string) => void })) => {
  
  const deleteStream = useAction('DELETE', `/api/streams/${props.stream}`, () => props.onDelete(props.stream));
  
  return (
    <React.Fragment>
      <div className="mt-2 mb-2">
        <h5>Delete Stream `{props.stream}`</h5>
      </div>
      <div className="mt-2 mb-2">
        <Alert color="warning">
          <h4 className="alert-heading">Warning</h4>
          <p>Deleting a stream is an irrecoverable operation. Aggregail does not implement any logic to inform other 
            applications of deleted streams, and as such the read models built from the event store can contain 
            information which is no longer present.</p>
          <p>Are you sure you want to delete the stream?</p>
        </Alert>
      </div>
      <div className="mt-2 mb-2">
        <Button
          type="button"
          color="secondary"
          className="mr-2"
          onClick={() => props.onCancel(props.stream)}
        >Cancel</Button>
        <Button
          type="button"
          color="danger"
          onClick={() => deleteStream()}
        >Delete</Button>
      </div>
    </React.Fragment>
  );
}

export default DeleteStreamPage;