import React, { useState } from 'react';

const SignIn = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSignIn = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await fetch('/api/identity', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        throw new Error('Sign in failed');
      }

      const data = await response.json();

      localStorage.setItem('token', data.token);

      window.location.href = '/';
    } catch (err) {
      setError(err.message);
    }
  };

  return (
      <div className="flex items-center justify-center min-h-screen bg-gray-100">
        <div className="w-full max-w-md p-8 space-y-6 bg-white rounded-lg shadow-md">
          <h2 className="text-2xl font-bold text-center">Sign In</h2>
          {error && <p className="text-red-500 text-center">{error}</p>}
          <form onSubmit={handleSignIn}>
            <div>
              <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-700">Email</label>
              <input
                  type="email"
                  id="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <div>
              <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-700">Password</label>
              <input
                  type="password"
                  id="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
            <button
                type="submit"
                className="w-full py-2 mt-4 text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none"
            >
              Sign In
            </button>
          </form>
          <p className="mt-4 text-center">
            Don't have an account? <a href="/sign-up" className="text-blue-600 hover:underline">Sign Up</a>
          </p>
        </div>
      </div>
  );
};

export default SignIn;
