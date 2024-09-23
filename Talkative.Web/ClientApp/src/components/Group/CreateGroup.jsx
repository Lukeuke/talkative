import { useState } from "react";
import {gql, useMutation, useQuery} from "@apollo/client";
import {refreshNavMenu} from "../NavMenu";

const CREATE_ROOM_MUTATION = gql`
  mutation CreateRoom($name: String!) {
    createRoom(name: $name) {
      id
      name
      ownerId
    }
  }
`;

const GET_ALL_ROOMS = gql`
  query {
    allRooms {
      id
      name
      ownerId
    }
  }
`;

export default function CreateGroup() {
  const [groupName, setGroupName] = useState("");
  const { refetch: refetchRooms } = useQuery(GET_ALL_ROOMS);
  const [createRoom, { loading, error, data }] = useMutation(CREATE_ROOM_MUTATION, {
    onCompleted: (data) => {
      refreshNavMenu(refetchRooms);
    },
    variables: { name: groupName },
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await createRoom();
    } catch (e) {
      console.error("Error creating group:", e);
    }
  };

  return (
      <div className="w-full max-w-xs">
        <form onSubmit={handleSubmit} className="flex flex-col gap-2">
          <input
              type="text"
              placeholder="Group Name"
              value={groupName}
              onChange={(e) => setGroupName(e.target.value)}
              className="w-full rounded-full px-4 py-2 text-black bg-MainSemiLight text-white"
              required
          />
          <button
              type="submit"
              className="rounded-full bg-white/10 px-10 py-3 font-semibold transition hover:bg-white/20"
              disabled={loading}
          >
            {loading ? "Creating..." : "Create"}
          </button>
        </form>

        {error && (
            <p className="text-red-500 mt-2">Failed to create group: {error.message}</p>
        )}

        {data && (
            <p className="text-green-500 mt-2">Group "{data.createRoom.name}" created successfully!</p>
        )}
      </div>
  );
}
