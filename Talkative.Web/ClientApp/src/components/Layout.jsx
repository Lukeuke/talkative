import React from 'react';
import { NavMenu } from './NavMenu';
import ApplicationManager from "./ApplicationManager";

export const Layout = ({ children }) => {
  return (
      <div className="flex h-screen">
        <ApplicationManager />
        <NavMenu />
        <div className="flex-grow bg-MainSemiLight">
          {children}
        </div>
      </div>
  );
};
