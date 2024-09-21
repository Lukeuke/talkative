import React from 'react';
import { NavMenu } from './NavMenu';

export const Layout = ({ children }) => {
  return (
      <div className="flex h-screen">
        <NavMenu />
        <div className="flex-grow bg-MainSemiLight">
          {children}
        </div>
      </div>
  );
};
