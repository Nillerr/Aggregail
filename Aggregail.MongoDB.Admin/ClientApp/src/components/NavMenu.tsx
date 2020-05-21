import * as React from 'react';
import {Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink} from 'reactstrap';
import {Link} from 'react-router-dom';
import './NavMenu.css';
import {useState} from "react";

const NavMenu = () => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3" light>
        <Container>
          <NavbarBrand tag={Link} to="/">Aggregail MongoDB</NavbarBrand>
          <NavbarToggler onClick={() => setIsOpen(!isOpen)} className="mr-2"/>
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={isOpen} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} to="/dashboard" disabled={true}>Dashboard</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/streams">Stream Browser</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/users" disabled={true}>Users</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} to="/logout" disabled={true}>Sign out</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
}

export default NavMenu;