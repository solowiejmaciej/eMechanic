import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingPage from './pages/LandingPage';
import HomePage from './pages/HomePage';
import LoginPage from './pages/auth/LoginPage';
import RegisterUserPage from './pages/auth/RegisterUserPage';
import RegisterWorkshopPage from './pages/auth/RegisterWorkshopPage';
import FindWorkshopPage from './pages/FindWorkshopPage';
import ForMechanicsPage from './pages/ForMechanicsPage';
import HowItWorksPage from './pages/HowItWorksPage';
import TermsPage from './pages/TermsPage';
import PrivacyPage from './pages/PrivacyPage';

import AboutPage from './pages/AboutPage';
import ContactPage from './pages/ContactPage';
import HelpPage from './pages/HelpPage';
import CookiesPage from './pages/CookiesPage';

import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';

function App() {
  return (
    <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID || "PLACEHOLDER_CLIENT_ID"}>
      <AuthProvider>
        <Router>
          <Routes>
            <Route path="/" element={<LandingPage />} />
            <Route
              path="/home"
              element={
                <ProtectedRoute>
                  <HomePage />
                </ProtectedRoute>
              }
            />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterUserPage />} />
            <Route path="/register-workshop" element={<RegisterWorkshopPage />} />
            <Route path="/find-workshop" element={<FindWorkshopPage />} />
            <Route path="/for-mechanics" element={<ForMechanicsPage />} />
            <Route path="/how-it-works" element={<HowItWorksPage />} />
            <Route path="/terms" element={<TermsPage />} />
            <Route path="/privacy" element={<PrivacyPage />} />
            <Route path="/about" element={<AboutPage />} />
            <Route path="/contact" element={<ContactPage />} />
            <Route path="/help" element={<HelpPage />} />
            <Route path="/cookies" element={<CookiesPage />} />
          </Routes>
        </Router>
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}

export default App;
