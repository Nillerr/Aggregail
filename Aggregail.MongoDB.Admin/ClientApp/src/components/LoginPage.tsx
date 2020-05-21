import React, {useState} from "react";
import {Button, Form, Input} from "reactstrap";
import logo from '../assets/logo.svg';
import Axios from "axios";

const LoginPage = (props: { onSignIn: () => void }) => {

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const signIn = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    const data = new FormData();
    data.set('username', username);
    data.set('password', password);

    Axios.post('/api/auth/login', data)
      .then(() => props.onSignIn())
      .catch(reason => console.error(reason));
  };

  return (
    <div className="d-flex flex-column justify-content-center align-items-center h-100">
      <Form className="shadow rounded bg-info text-center" style={{width: '22rem', padding: '2.25rem'}}
            onSubmit={signIn}>
        <img className="mb-4" src={logo} alt="thingthing"/>
        <Input placeholder="Username" bsSize="sm" value={username}
               onChange={e => setUsername(e.target.value)}/>
        <Input className="mt-3" placeholder="Password" bsSize="sm" value={password}
               onChange={e => setPassword(e.target.value)}/>
        <Button color="primary" className="border-0 mt-4 w-100">Sign in</Button>
      </Form>
    </div>
  );
};

export default LoginPage;