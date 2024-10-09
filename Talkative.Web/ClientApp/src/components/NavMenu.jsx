import React, {useEffect, useState} from 'react';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import HomeSvg from '../images/home.svg'
import GroupSidebarComponent from "./Group/GroupSidebarComponent";
import { useQuery, gql } from '@apollo/client';
import {
  getUnreadGroupsLocalStorage,
  removeUnreadGroupLocalStorage,
  setUnreadGroupsLocalStorage
} from "../helpers/RoomUnreadStatusMock";
import UserProfile from "./UserProfile";

const GET_ALL_ROOMS = gql`
  query {
    allRooms {
      id
      name
      imageUrl
      ownerId
    }
  }
`;

export const NavMenu = () => {

  const { loading, error, data } = useQuery(GET_ALL_ROOMS);

  const [unreadGroups, setUnreadGroups] = useState([]);

  const [refreshCounter, setRefreshCounter] = useState(0);
  
  const [isOpen, setIsOpen] = useState(false);

  const refreshNavbar = () => {
    console.log('refreshing navbar')
    setRefreshCounter((prev) => prev + 1);
  };
  
  useEffect(() => {
    const fetchUnreadGroups = async () => {
      const storedUnreadGroups = await getUnreadGroupsLocalStorage();
      setUnreadGroups(storedUnreadGroups);
    };
    fetchUnreadGroups();
  }, []);

  const [startTouchX, setStartTouchX] = useState(0);
  const [endTouchX, setEndTouchX] = useState(0);
  
  const handleTouchStart = (e) => {
    setStartTouchX(e.touches[0].clientX); // Get initial touch position
  };

  const handleTouchMove = (e) => {
    setEndTouchX(e.touches[0].clientX); // Update touch position
  };

  const handleTouchEnd = () => {
    const threshold = 50; // Minimum swipe distance to trigger action
    if (startTouchX + threshold < endTouchX) {
      // Swipe right
      setIsOpen(true);
    } else if (startTouchX - threshold > endTouchX) {
      // Swipe left
      setIsOpen(false);
    }
  };
  
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
    return null;
  }

  return (
      <div className="h-screen w-[80px] bg-[#313137] text-white flex flex-col items-center justify-between">
        <div className='py-[10px]'>
          <Link href={"/"} to={"/"}>
            <img src={HomeSvg} alt="Home" />
          </Link>
        </div>

        <nav className="flex flex-col space-y-[10px] pt-[10px] flex-1">
          {data.allRooms.map((group) => {
            return (
                <GroupSidebarComponent
                    key={group.id}
                    {...group}
                    id={group.id}
                    unread={unreadGroups.find(e => e === group.id)}
                    refreshNavbar={refreshNavbar}
                />
            );
          })}
        </nav>

        <div className="py-[10px]">
          <UserProfile />
        </div>
      </div>
  );
};

export const refreshNavMenu = (refetch) => {
  refetch();
};

export const setUnreadGroupsHook = async (groupId, isRead) => {
  if (isRead) {
    await removeUnreadGroupLocalStorage(groupId);
  } else {
    await setUnreadGroupsLocalStorage(groupId);
  }
};