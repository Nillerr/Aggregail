import React from "react";
import {Light as SyntaxHighlighter} from "react-syntax-highlighter";
import json from 'react-syntax-highlighter/dist/esm/languages/hljs/json';
import style from 'react-syntax-highlighter/dist/esm/styles/hljs/idea';

SyntaxHighlighter.registerLanguage('json', json);

const Json = (props: React.PropsWithChildren<{}>) => (
  <SyntaxHighlighter language="json" style={style}>
    {props.children}
  </SyntaxHighlighter>
);

export default Json;