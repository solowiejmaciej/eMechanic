import React, { createContext, useState, useContext, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import { login as apiLogin, googleLogin as apiGoogleLogin, logout as apiLogout, getCurrentUser } from '../api/auth';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Define logout here so it can be used by useEffect
  const logout = () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    setUser(null);
    // Optionally, redirect to login page if this logout is triggered by an internal event like token expiration
    // For external logout actions (e.g., user clicking a button), the redirect can be handled in the component.
    // window.location.href = '/login'; // Keep this commented for internal use, uncomment for full redirect
  };

  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('accessToken');
      if (token) {
        try {
          // Check if token is expired
          const decoded = jwtDecode(token);
          const currentTime = Date.now() / 1000;
          
          if (decoded.exp < currentTime) {
            // Token expired, try to refresh or logout
            // For now, let's just logout
            logout();
          } else {
            // Token valid, fetch user data
            try {
              const userData = await getCurrentUser();
              setUser(userData);
            } catch (err) {
              console.error("Failed to fetch user data:", err);
              // If fetching user fails (e.g. 401), logout
              logout();
            }
          }
        } catch (error) {
          // Invalid token (e.g., malformed JWT)
          console.error("Invalid token format:", error);
          logout();
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (email, password) => {
    try {
      const data = await apiLogin(email, password);
      // Support both 'token' and 'accessToken' properties from API
      const token = data.token || data.accessToken;
      
      if (!token) {
        throw new Error("Invalid response from server: missing token");
      }

      localStorage.setItem('accessToken', token);
      localStorage.setItem('refreshToken', data.refreshToken);
      
      // Fetch user data after successful login
      const userData = await getCurrentUser();
      setUser(userData);
      
      return { success: true };
    } catch (error) {
      console.error("Login failed:", error);
      return { success: false, error: error.response?.data?.detail || error.response?.data?.title || "Login failed" };
    }
  };

  const googleLogin = async (token) => {
    try {
      const data = await apiGoogleLogin(token);
      const accessToken = data.token;

      if (!accessToken) {
        throw new Error("Invalid response from server: missing token");
      }

      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', data.refreshToken);
      
      // Fetch user data after successful login
      const userData = await getCurrentUser();
      setUser(userData);
      
      return { success: true };
    } catch (error) {
      console.error("Google login failed:", error);
      return { success: false, error: error.response?.data?.detail || error.response?.data?.title || error.message || "Google login failed" };
    }
  };

  const handleLogout = async () => {
    try {
      await apiLogout();
    } catch (error) {
      console.error("API Logout error:", error);
    } finally {
      logout(); // Clear local state
      window.location.href = '/login'; // Redirect after full logout
    }
  };

  return (
    <AuthContext.Provider value={{ user, login, googleLogin, logout, loading }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
