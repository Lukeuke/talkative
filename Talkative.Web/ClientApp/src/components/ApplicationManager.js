import {gql, useMutation} from "@apollo/client";
import {useEffect} from "react";
import {useLocation, useNavigate} from "react-router-dom";

const USER_STATUS = gql`
  mutation SetUserStatus($isOnline: Boolean!) {
    setUserStatus(isOnline: $isOnline)
  }
`;

export default function ApplicationManager() {
    const [setUserStatus] = useMutation(USER_STATUS);

  const navigate = useNavigate();
    
    useEffect(() => {
      setUserStatus({variables: {isOnline: true}})
    }, [])
    
    useEffect(() => {
      const intervalId = setInterval(() => {
        setUserStatus({variables: {isOnline: true}})
            .then(response => {
              // console.log("User status updated:", response);
            })
            .catch(error => {
              console.error("Error updating user status:", error);
              
              if (error.message.includes("not authorized")) {
                navigate("/sign-in");
              }
            });
      }, 15000);

      return () => clearInterval(intervalId);
    }, [setUserStatus]);
  
  return (
      <></>
  )
}