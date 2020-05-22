import React from "react";
import {useAction, useRequest} from "../hooks";
import {User} from "../model";
import {Button, ButtonGroup, Table} from "reactstrap";
import {LoadableTableBody} from "./LoadableTableBody";

const UserDetails = (props: { user: User }) => {
  return (
    <React.Fragment>
    <tr>
      <td>Username</td>
      <td>{props.user.username}</td>
    </tr>
    <tr>
      <td>Full Name</td>
      <td>{props.user.fullName}</td>
    </tr>
    </React.Fragment>
  );
};

const UserPage = (props: { id: string, onDelete: () => void, onBack: () => void }) => {
  const userState = useRequest<User>(`/api/users/${props.id}`);

  const deleteUser = useAction('DELETE', `/api/users/${props.id}`, props.onDelete);

  return (
    <React.Fragment>
      <div className="d-flex mb-4">
        <h5 className="mr-auto">User Details</h5>
        <div>
          <ButtonGroup>
            <Button type="button" color="secondary" outline={true} disabled={true}>Edit</Button>
            <Button type="button" color="secondary" outline={true} disabled={true}>Change Password</Button>
            <Button type="button" color="danger" outline={true} onClick={deleteUser}>Delete</Button>
            <Button type="button" color="secondary" outline={true} onClick={props.onBack}>Back</Button>
          </ButtonGroup>
        </div>
      </div>
      <Table bordered={true} size="sm">
        <thead className="thead-dark">
        <tr>
          <th scope="col">Fields</th>
          <th scope="col">Values</th>
        </tr>
        </thead>
        <LoadableTableBody
          colSpan={2}
          state={userState}
          render={({data: user}) => <UserDetails user={user}/>}
        />
      </Table>
    </React.Fragment>
  );
};

export default UserPage;