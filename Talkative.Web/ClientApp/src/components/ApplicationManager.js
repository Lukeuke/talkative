import {gql, useMutation} from "@apollo/client";
import {useEffect} from "react";

const USER_STATUS = gql`
  mutation SetUserStatus($isOnline: Boolean!) {
    setUserStatus(isOnline: $isOnline)
  }
`;

export default function ApplicationManager() {
    const [setUserStatus] = useMutation(USER_STATUS);

    useEffect(() => {
      setUserStatus({variables: {isOnline: true}})
    }, [])
    
    useEffect(() => {
      const intervalId = setInterval(() => {
        setUserStatus({variables: {isOnline: true}})
            .then(response => {
              console.log("User status updated:", response);
            })
            .catch(error => {
              console.error("Error updating user status:", error);
            });
      }, 15000); // every 15 seconds

      return () => clearInterval(intervalId);
    }, [setUserStatus]);
  
  return (
      <></>
  )
}