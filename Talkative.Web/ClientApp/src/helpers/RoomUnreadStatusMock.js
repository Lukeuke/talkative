export const setUnreadGroupsLocalStorage = (groupId) => {
  const unreadGroups = JSON.parse(localStorage.getItem('unreadGroups')) || [];
  if (!unreadGroups.includes(groupId)) {
    unreadGroups.push(groupId);
    localStorage.setItem('unreadGroups', JSON.stringify(unreadGroups));
  }
};

export const removeUnreadGroupLocalStorage = (groupId) => {
  let unreadGroups = JSON.parse(localStorage.getItem('unreadGroups')) || [];
  unreadGroups = unreadGroups.filter((id) => id !== groupId);
  localStorage.setItem('unreadGroups', JSON.stringify(unreadGroups));
};

export const getUnreadGroupsLocalStorage = () => {
  return JSON.parse(localStorage.getItem('unreadGroups')) || [];
};
