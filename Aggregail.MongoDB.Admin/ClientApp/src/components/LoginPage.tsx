import React, {useState} from "react";
import {Button, Form, Input} from "reactstrap";
import logo from '../assets/logo.svg';

const LoginPage = (props: { onSignIn: (server: string) => void }) => {

  const [server, setServer] = useState('https://localhost:5001');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  return (
    <div className="d-flex flex-column justify-content-center align-items-center h-100">
      <Form className="shadow rounded bg-info text-center" style={{width: '22rem', padding: '2.25rem'}} onSubmit={() => props.onSignIn(server)}>
        <img className="mb-4" src={logo} alt="thingthing"/>
        <Input placeholder="https://localhost:5001" bsSize="sm" value={server} onChange={e => setServer(e.target.value)}/>
        <Input className="mt-4" placeholder="Username" bsSize="sm" value={username} onChange={e => setUsername(e.target.value)}/>
        <Input className="mt-3" placeholder="Password" bsSize="sm" value={password} onChange={e => setPassword(e.target.value)}/>
        <Button color="primary" className="border-0 mt-4 w-100">Sign in</Button>
      </Form>
    </div>
  );
};

export default LoginPage;