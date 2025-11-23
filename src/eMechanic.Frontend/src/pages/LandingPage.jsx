import React from 'react';
import Navbar from '../components/layout/Navbar';
import HeroSection from '../features/landing/HeroSection';
import FeaturesSection from '../features/landing/FeaturesSection';

const LandingPage = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors">
      <Navbar />
      <main>
        <HeroSection />
        <FeaturesSection />
      </main>
      {/* Footer placeholder */}
      <footer className="bg-transparent text-slate-400 py-12 text-center">
        <p>© 2024 eMechanic. All rights reserved.</p>
      </footer>
    </div>
  );
};

export default LandingPage;
