import React, {useState} from "react";
import {Dropdown, DropdownItem, DropdownMenu, DropdownToggle} from "reactstrap";

export interface AppTheme {
  name: string;
  style: 'light' | 'dark';
}

const themes: AppTheme[] = [
  {name: 'bootstrap', style: 'light'},
  {name: 'cerulean', style: 'light'},
  {name: 'cosmo', style: 'light'},
  {name: 'cyborg', style: 'dark'},
  {name: 'darkly', style: 'dark'},
  {name: 'flatly', style: 'light'},
  {name: 'journal', style: 'light'},
  {name: 'litera', style: 'light'},
  {name: 'lumen', style: 'light'},
  {name: 'lux', style: 'light'},
  {name: 'materia', style: 'light'},
  {name: 'minty', style: 'light'},
  {name: 'pulse', style: 'light'},
  {name: 'sandstone', style: 'light'},
  {name: 'simplex', style: 'light'},
  {name: 'sketchy', style: 'light'},
  {name: 'slate', style: 'dark'},
  {name: 'solar', style: 'dark'},
  {name: 'spacelab', style: 'light'},
  {name: 'superhero', style: 'dark'},
  {name: 'united', style: 'light'},
  {name: 'yeti', style: 'light'},
];

const capitalize = (string: string) => string.charAt(0).toUpperCase() + string.substr(1);

const ThemeSelector = (props: { fill?: boolean, color?: string, onChange?: (theme: AppTheme) => void }) => {
  const [isOpen, setIsOpen] = useState(false);
  const toggle = () => setIsOpen(prevState => !prevState);
  
  const currentThemeName = localStorage.getItem('theme') ?? 'bootstrap';
  
  const setTheme = (theme: AppTheme) => {
    localStorage.setItem('theme', theme.name);
    localStorage.setItem('theme-style', theme.style);
    (window as any).$appUpdateTheme();
    
    if (props.onChange) {
      props.onChange(theme);
    }
  };
  
  return (
    <Dropdown className={props.fill ? 'd-flex' : ''} isOpen={isOpen} toggle={toggle}>
      <DropdownToggle className={props.fill ? 'flex-grow-1' : ''} caret color={props.color}>Theme</DropdownToggle>
      <DropdownMenu>
        {themes.map(theme => 
          <DropdownItem key={theme.name} onClick={() => setTheme(theme)} disabled={currentThemeName === theme.name}>
            {currentThemeName === theme.name
              ? <strong>{capitalize(theme.name)} ({capitalize(theme.style)})</strong>
              : `${capitalize(theme.name)} (${capitalize(theme.style)})`
            }
          </DropdownItem>
        )}
      </DropdownMenu>
    </Dropdown>
  );
};

export default ThemeSelector;