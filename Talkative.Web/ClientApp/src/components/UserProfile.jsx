import React, {useEffect, useState} from 'react';
import {cacheImage, getCachedImage} from "../helpers/CacheHelper";
const UserProfile = () => {
  const [showLogoutModal, setShowLogoutModal] = useState(false);
  const [uniqueName, setUniqueName] = useState(null);
  const [id, setId] = useState(null);
  const [cachedImageUrl, setCachedImageUrl] = useState(null);
  const parseJwt = (token) => {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
          atob(base64)
              .split('')
              .map(function (c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
              })
              .join('')
      );
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Error parsing JWT:', error);
      return null;
    }
  };

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      const payload = parseJwt(token);
      if (payload && payload.unique_name) {
        setUniqueName(payload.unique_name);
        setId(payload.id);
      }
    }
  }, []);


  useEffect(() => {
    const fetchAndCacheImage = async () => {
      if (id) {
        const imageUrl = `${window.location.origin}/api/cdn/user/${id}`;
        const cachedUrl = await getCachedImage(imageUrl);
        if (cachedUrl) {
          setCachedImageUrl(cachedUrl);
        } else {
          await cacheImage(imageUrl);
          setCachedImageUrl(imageUrl);
        }
      }
    };

    fetchAndCacheImage();
  }, [id]);
  
  const handleProfileClick = () => {
    setShowLogoutModal(true);
  };

  const closeModal = () => {
    setShowLogoutModal(false);
  };
  
  const handleLogout = () => {
    localStorage.removeItem('token');
    window.location.href = '/sign-in';
  };
  
  return (
      <div>
        <div
            className="relative flex flex-col items-center group"
            onClick={handleProfileClick}
        >
          <a
              className="flex flex-col items-center rounded-full h-[50px] w-[50px] c-pointer"
          >
            <img
                src={cachedImageUrl}
                alt="User profile"
                className="rounded-full h-full w-full"
            />
          </a>
          <span className="absolute left-[60px] bg-gray-700 text-white text-xs rounded-md px-2 py-1 opacity-0 group-hover:opacity-100 transition-opacity duration-300 z-50">
            {uniqueName}
          </span>
        </div>
        
        {showLogoutModal && (
            <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
              <div className="bg-SemiDark rounded-md p-6 w-[300px] shadow-lg">
                <h2 className="text-lg font-semibold mb-4">Confirm Logout</h2>
                <p>Are you sure you want to log out?</p>
                <div className="flex justify-end space-x-4 mt-6">
                  <button
                      onClick={closeModal}
                      className="bg-gray-200 text-gray-700 px-4 py-2 rounded-md hover:bg-gray-300"
                  >
                    Cancel
                  </button>
                  <button
                      onClick={handleLogout}
                      className="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600"
                  >
                    Logout
                  </button>
                </div>
              </div>
            </div>
        )}
      </div>
  );
};

export default UserProfile;
