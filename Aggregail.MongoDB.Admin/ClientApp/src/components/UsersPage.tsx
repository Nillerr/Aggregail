import React from "react";
import {Alert, Button, Spinner, Table} from "reactstrap";
import {Link} from "react-router-dom";
import {usePolling} from "../hooks";
import {User} from "../model";

const UserRow = (props: { user: User }) => {
  return (
    <tr>
      <td><Link to={`/users/${props.user.id}`}>{props.user.username}</Link></td>
      <td>{props.user.fullName}</td>
    </tr>
  );
};

const UsersPage = () => {

  const usersState = usePolling<User[], { users: User[] }>('/api/users', users => ({users}));

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
        <tbody>
        {usersState.kind === "Loading"
          ? <tr>
            <td colSpan={2} className="text-center">
              <Spinner>Loading...</Spinner>
            </td>
          </tr>
          : null
        }
        {usersState.kind === 'Failed'
          ? <tr>
            <td colSpan={2}>
              <Alert color="danger">{`${usersState.reason}`}</Alert>
            </td>
          </tr>
          : null
        }
        {usersState.kind === 'Loaded'
          ? usersState.users.map(user => <UserRow key={user.id} user={user}/>)
          : null
        }
        </tbody>
      </Table>
    </React.Fragment>
  );
};

export default UsersPage;