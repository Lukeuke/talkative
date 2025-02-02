import React, {useEffect, useState} from 'react';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import HomeSvg from '../images/home.svg'
import GroupSidebarComponent from "./Group/GroupSidebarComponent";
import {useQuery, gql, useMutation} from '@apollo/client';
import {
  getUnreadGroupsLocalStorage,
  removeUnreadGroupLocalStorage,
  setUnreadGroupsLocalStorage
} from "../helpers/RoomUnreadStatusMock";
import UserProfile from "./UserProfile";
import {useNavigate} from "react-router-dom";
import DeleteConfirmation from "./Shared/DeleteConfirmation";

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
  const navigate = useNavigate();
  
  const { loading, error, data } = useQuery(GET_ALL_ROOMS);

  const [unreadGroups, setUnreadGroups] = useState([]);

  const [refreshCounter, setRefreshCounter] = useState(0);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const [contextMenu, setContextMenu] = useState({ visible: false, x: 0, y: 0, groupId: null });

  const DELETE_ROOM = gql`
    mutation DeleteRoom($roomId: UUID!) {
      deleteRoom(roomId: $roomId)
    }
  `;

  const { refetch: refetchRooms } = useQuery(GET_ALL_ROOMS);
  
  const [deleteRoom] = useMutation(DELETE_ROOM, {
    onCompleted: () => {
      console.log('Room deleted successfully');
      refreshNavMenu(refetchRooms);
    },
    onError: (error) => {
      console.error('Error deleting room:', error);
    },
  });
  
  const handleContextMenu = (e, groupId, roomName) => {
    e.preventDefault();

    setContextMenu({
      visible: true,
      x: e.pageX,
      y: e.pageY,
      groupId: groupId,
      roomName: roomName
    });
  };

  const handleCloseMenu = () => {
    setContextMenu({ ...contextMenu, visible: false });
  };

  const handleEdit = () => {
    handleCloseMenu();
  };

  const handleDelete = () => {
    setIsModalOpen(true);
  };

  const confirmDelete = () => {
    const roomId = contextMenu.groupId;
    deleteRoom({ variables: { roomId } });
    handleCloseMenu();
  };
  
  const handleOpen = () => {
    navigate(`/group/${contextMenu.groupId}`);
    handleCloseMenu();
  };
  
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
      <>
        <div className="h-screen w-[80px] bg-[#313137] text-white flex flex-col items-center justify-between">
          <div className='py-[10px]'>
            <Link href={"/"} to={"/"}>
              <img src={HomeSvg} alt="Home" />
            </Link>
          </div>
  
          <nav className="flex flex-col space-y-[10px] pt-[10px] flex-1"
               onContextMenu={(e) => {
                  e.preventDefault();
                  console.log(e.target)
          }}>
            {data.allRooms.map((group) => {
              return (
                  <div
                      key={group.id}
                      onContextMenu={(e) => handleContextMenu(e, group.id, group.name)}
                  >
                    <GroupSidebarComponent
                        key={group.id}
                        {...group}
                        id={group.id}
                        unread={unreadGroups.find(e => e === group.id)}
                        refreshNavbar={refreshNavbar}
                    />
                  </div>
              );
            })}
          </nav>
  
          {contextMenu.visible && (
              <div
                  className="absolute z-50 bg-MainDark border border-gray-300 shadow-lg"
                  style={{ top: contextMenu.y, left: contextMenu.x }}
                  onMouseLeave={handleCloseMenu}
              >
                <ul>
                  <li onClick={handleOpen} className="p-2 hover:bg-SemiDark cursor-pointer">Open</li>
                  <li onClick={handleEdit} className="p-2 hover:bg-SemiDark cursor-pointer">Edit</li>
                  <li onClick={handleDelete} className="p-2 hover:bg-SemiDark0 cursor-pointer text-red-500">Delete</li>
                </ul>
              </div>
          )}
          
          <div className="py-[10px] border-t-2 border-gray-500">
            <UserProfile />
          </div>
        </div>
        <DeleteConfirmation
            isOpen={isModalOpen}
            name={contextMenu.roomName}
            onClose={() => setIsModalOpen(false)}
            onConfirm={confirmDelete}
        />
      </>
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