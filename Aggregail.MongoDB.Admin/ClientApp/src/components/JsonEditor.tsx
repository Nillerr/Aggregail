import {Input} from "reactstrap";
import React from "react";

const JsonEditor = (props: { value: string, onValueChange: (value: string) => void }) => {
  return (
    <Input
      type="textarea"
      className="input-code bg-dark text-light"
      value={props.value}
      onChange={(e) => props.onValueChange(e.target.value)}
    />
  );
};

export default JsonEditor;