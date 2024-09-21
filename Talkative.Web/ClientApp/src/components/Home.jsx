import React from 'react';
import CreateGroup from "./Group/CreateGroup";

export const Home = () => {
  return (
      <main className="flex min-h-screen text-white">
        <div className="grid grid-cols-1 md:grid-cols-3 w-full h-[100vh]">
          <div className="bg-gray-800 h-full p-4 flex justify-center">Column 1</div>
          <div className="bg-gray-700 h-full p-4 flex flex-col items-center justify-center">
            <div className="text-[2rem] mb-4">Create a Group</div>
            <CreateGroup />
          </div>
          <div className="bg-gray-600 h-full p-4 flex justify-center">Column 3</div>
        </div>
      </main>
  );
};
