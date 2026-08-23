import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';


import ChakraHero from './features/landing/ChakraHero';
import ChakraLandingPage from './pages/ChakraLandingPage';
import ChakraFeature from './features/landing/ChakraFeature';
import ChakraHelpPage from './pages/ChakraHelpPage';


import { GoogleOAuthProvider } from '@react-oauth/google';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/auth/ProtectedRoute';
import ChakraContact from './pages/ChakraContact';
import ChakraCookies from './pages/ChakraCookies';
import ChakraAbout from './pages/ChakraAbout';
import ChakraPrivacy from './pages/ChakraPrivacy';
import ChakraFindWorshop from './pages/ChakraFindWorkshop';
import ChakraHowItWorks from './pages/ChakraHowItWorks';
import CharkaForMechanics from './pages/ChakraForMechanics';
import ChakraTerms from './pages/ChakraTerms';
import ChakraLogin from './pages/auth/ChakraLogin';
import ChakraRegister from './pages/auth/ChakraRegisterPage';
import ChakraHomePage from './pages/ChakraHomePage';
import WorkshopDashboard from './pages/WorkshopDashboard';


function App() {
  return (
    <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID || "PLACEHOLDER_CLIENT_ID"}>
      <AuthProvider>
        <Router>
          <Routes>
            <Route path="/" element={<ChakraLandingPage />} />
            <Route
              path="/home"
              element={
               <ProtectedRoute>
                  <ChakraHomePage />
                </ProtectedRoute>
             }
            />
            <Route path= "/help" element={<ChakraHelpPage/>} />
            <Route path= "/contact" element={<ChakraContact/>} />
            <Route path="/cookies" element={<ChakraCookies/>} />
            <Route path="/about" element={<ChakraAbout />} />
            <Route path="/privacy" element={<ChakraPrivacy />} />
            <Route path="/find-workshop" element={<ChakraFindWorshop/>}/>
            <Route path="/how-it-works" element={<ChakraHowItWorks/>}/>ś
            <Route path="/for-mechanics" element ={<CharkaForMechanics/>}/>
            <Route path="/terms" element={<ChakraTerms/>}/>
            <Route path="/login" element={<ChakraLogin/>}/>
            <Route path="/register" element={<ChakraRegister/>}/>
            <Route path="/chakraworkshopdashboard" element={<WorkshopDashboard/>} />
          </Routes>
        </Router>
      </AuthProvider>
    </GoogleOAuthProvider>
  );
}

export default App;
