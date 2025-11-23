import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingPage from './pages/LandingPage';
import LoginPage from './pages/auth/LoginPage';
import RegisterUserPage from './pages/auth/RegisterUserPage';
import RegisterWorkshopPage from './pages/auth/RegisterWorkshopPage';
import FindWorkshopPage from './pages/FindWorkshopPage';
import ForMechanicsPage from './pages/ForMechanicsPage';
import HowItWorksPage from './pages/HowItWorksPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterUserPage />} />
        <Route path="/register-workshop" element={<RegisterWorkshopPage />} />
        <Route path="/find-workshop" element={<FindWorkshopPage />} />
        <Route path="/for-mechanics" element={<ForMechanicsPage />} />
        <Route path="/how-it-works" element={<HowItWorksPage />} />
      </Routes>
    </Router>
  );
}

export default App;
