import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import HomeSvg from '../images/home.svg'
import GroupSidebarComponent from "./Group/GroupSidebarComponent";
import { useQuery, gql } from '@apollo/client';

const GET_ALL_ROOMS = gql`
  query {
    allRooms {
      id
      name
      ownerId
    }
  }
`;

export const NavMenu = () => {

  const { loading, error, data } = useQuery(GET_ALL_ROOMS);
  
  if (loading) {
    return (
        <div className="h-screen w-[80px] bg-[#313137] text-white flex flex-col items-center">
          <div className='py-[10px]'>
            <Link href={"/"} to={"/"}>
              <img src={HomeSvg} alt="Home" />
            </Link>
          </div>
          <nav className="flex flex-col space-y-[10px] pt-[10px]">
            
          </nav>
        </div>
    );
  }
  if (error) {
    console.error(error.message);
    return null;
  }

  return (
      <div className="h-screen w-[80px] bg-[#313137] text-white flex flex-col items-center">
        <div className='py-[10px]'>
          <Link href={"/"} to={"/"}>
            <img src={HomeSvg} alt="Home" />
          </Link>
        </div>
        <nav className="flex flex-col space-y-[10px] pt-[10px]">
          {data.allRooms.map((group) => (
              <GroupSidebarComponent key={group.id} {...group} id={group.id} />
          ))}

          {/*<CreateGroupPopup />*/}
        </nav>
      </div>
  );
};
