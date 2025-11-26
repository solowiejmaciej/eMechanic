import React from 'react';
import Navbar from '../components/layout/Navbar';
import HeroSection from '../features/landing/HeroSection';
import FeaturesSection from '../features/landing/FeaturesSection';
import Footer from '../components/layout/Footer';

const LandingPage = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors">
      <Navbar />
      <main>
        <HeroSection />
        <FeaturesSection />
      </main>
      <Footer />
    </div>
  );
};

export default LandingPage;
