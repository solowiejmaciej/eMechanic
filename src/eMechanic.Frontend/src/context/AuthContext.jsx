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
    localStorage.removeItem('isMockSession');
    localStorage.removeItem('mockUser');
    setUser(null);
  };

  useEffect(() => {
    const initAuth = async () => {
      const isMock = localStorage.getItem('isMockSession');
      if (isMock === 'true') {
        try {
          const mockUser = JSON.parse(localStorage.getItem('mockUser'));
          setUser(mockUser);
          setLoading(false);
          return;
        } catch (e) {
          console.error("Failed to restore mock user session", e);
        }
      }

      const token = localStorage.getItem('accessToken');
      if (token) {
        try {
          // Check if token is expired
          const decoded = jwtDecode(token);
          const currentTime = Date.now() / 1000;
          
          if (decoded.exp < currentTime) {
            // Token expired, try to refresh or logout
            logout();
          } else {
            // Token valid, set user state according to identityType claim
            try {
              if (decoded.identityType === 'User') {
                const userData = await getCurrentUser();
                setUser({ ...userData, role: 'User' });
              } else if (decoded.identityType === 'Workshop') {
                setUser({
                  id: decoded.workshopId,
                  email: decoded.email || decoded.sub,
                  role: 'Workshop',
                  isWorkshop: true
                });
              } else {
                logout();
              }
            } catch (err) {
              console.error("Failed to fetch user data:", err);
              logout();
            }
          }
        } catch (error) {
          console.error("Invalid token format:", error);
          logout();
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (email, password, isWorkshop = false) => {
    try {
      const data = await apiLogin(email, password, isWorkshop);
      // Support both 'token' and 'accessToken' properties from API
      const token = data.token || data.accessToken;
      
      if (!token) {
        throw new Error("Invalid response from server: missing token");
      }

      localStorage.setItem('accessToken', token);
      localStorage.setItem('refreshToken', data.refreshToken || '');
      
      const decoded = jwtDecode(token);
      let sessionUser = null;
      if (decoded.identityType === 'User') {
        const userData = await getCurrentUser();
        sessionUser = { ...userData, role: 'User' };
      } else if (decoded.identityType === 'Workshop') {
        sessionUser = {
          id: decoded.workshopId,
          email: decoded.email || decoded.sub,
          role: 'Workshop',
          isWorkshop: true
        };
      }
      setUser(sessionUser);
      return { success: true };
    } catch (error) {
      const isLocalhost = window.location.hostname === "localhost" || window.location.hostname === "127.0.0.1";
      if (!isLocalhost) {
        console.error("API login failed on production:", error);
        return {
          success: false,
          error: error.response?.data?.detail || error.response?.data?.title || error.message || "Błąd logowania na serwerze."
        };
      }

      console.warn("API login failed, falling back to offline mock session.", error);
      
      let sessionUser = null;
      if (isWorkshop) {
        sessionUser = {
          id: "mock-workshop-id",
          email: email || "warsztat@example.com",
          displayName: "Auto Serwis eMechanic",
          city: "Kraków",
          address: "Pawia 15",
          phone: "+48 500 600 700",
          description: "Profesjonalny warsztat samochodowy z wieloletnim doświadczeniem. Oferujemy pełen zakres mechaniki pojazdowej, diagnostykę komputerową oraz serwis klimatyzacji.",
          role: 'Workshop',
          isWorkshop: true
        };
      } else {
        sessionUser = {
          id: "mock-user-id",
          email: email || "klient@example.com",
          firstName: "Jan",
          lastName: "Kowalski",
          role: 'User'
        };
      }
      
      localStorage.setItem('isMockSession', 'true');
      localStorage.setItem('mockUser', JSON.stringify(sessionUser));
      setUser(sessionUser);
      
      return { success: true };
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

  const refreshUser = async () => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      try {
        const decoded = jwtDecode(token);
        if (decoded.identityType === 'User') {
          const userData = await getCurrentUser();
          setUser({ ...userData, role: 'User' });
        }
      } catch (err) {
        console.error("Failed to refresh user data:", err);
      }
    }
  };

  return (
    <AuthContext.Provider value={{ user, login, googleLogin, logout, loading, refreshUser }}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
