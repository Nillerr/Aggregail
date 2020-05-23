import * as React from 'react';
import {useState} from 'react';
import {Button, Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink} from 'reactstrap';
import {Link} from 'react-router-dom';
import {useAction} from "../hooks";
import ThemeSelector, {AppTheme} from "./ThemeSelector";

const NavMenu = (props: { onSignOut: () => void, onChangeTheme?: (theme: AppTheme) => void }) => {
  const [isOpen, setIsOpen] = useState(false);
  
  const signOut = useAction('POST', '/api/auth/logout', props.onSignOut);

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3 navbar navbar-dark bg-primary">
        <Container>
          <NavbarBrand tag={Link} to="/">Aggregail MongoDB</NavbarBrand>
          <NavbarToggler onClick={() => setIsOpen(!isOpen)} className="mr-2"/>
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={isOpen} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <ThemeSelector onChange={props.onChangeTheme}/>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/dashboard" disabled={true}>Dashboard</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/streams">Stream Browser</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/users">Users</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Button} color="link" onClick={() => signOut()}>Sign out</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
}

export default NavMenu;