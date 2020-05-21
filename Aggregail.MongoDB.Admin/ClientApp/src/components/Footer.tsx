import React from "react";
import {Container} from "reactstrap";

const Footer = () => (
  <Container fluid={true} tag="footer" className="text-center mb-5 mt-5">
    Aggregail MongoDB Admin 1.0.0.0-alpha.1 · <a target="_blank" href="https://github.com/nillerr/Aggregail">Documentation</a>
  </Container>
);

export default Footer;