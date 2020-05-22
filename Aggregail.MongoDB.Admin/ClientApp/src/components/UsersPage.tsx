import React from "react";
import {Table} from "reactstrap";
import {Link} from "react-router-dom";
import {usePolling} from "../hooks";
import {User} from "../model";
import {LoadableTableBody} from "./LoadableTableBody";

const UserRow = (props: { user: User }) => {
  return (
    <tr>
      <td><Link to={`/users/${props.user.id}`}>{props.user.username}</Link></td>
      <td>{props.user.fullName}</td>
    </tr>
  );
};

const UsersPage = (props: {}) => {

  const usersState = usePolling<User[]>('/api/users');

  return (
    <React.Fragment>
      <div className="d-flex mb-4">
        <h5 className="mr-auto">Users</h5>
        <div>
          <Link className="ml-2 flex-shrink-0 text-nowrap btn btn-outline-secondary" to="/users/new">New User</Link>
        </div>
      </div>
      <Table bordered={true} size="sm">
        <thead className="thead-dark">
        <tr>
          <th scope="col">Login Name</th>
          <th scope="col">Full Name</th>
        </tr>
        </thead>
        <LoadableTableBody
          colSpan={2}
          state={usersState}
          render={({data: users}) => users.map(user => <UserRow key={user.id} user={user}/>)}
        />
      </Table>
    </React.Fragment>
  );
};

export default UsersPage;