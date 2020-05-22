import * as React from 'react';
import {Container} from 'reactstrap';
import NavMenu from './NavMenu';
import Footer from "./Footer";
import {AppTheme} from "./ThemeSelector";

const Layout = (props: { onSignOut: () => void, onChangeTheme?: (theme: AppTheme) => void, children?: React.ReactNode }) => (
  <React.Fragment>
    <NavMenu onSignOut={props.onSignOut} onChangeTheme={props.onChangeTheme}/>
    <Container>
      {props.children}
    </Container>
    <Footer/>
  </React.Fragment>
);

export default Layout;
