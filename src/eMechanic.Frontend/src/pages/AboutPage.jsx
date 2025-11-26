import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';

const AboutPage = () => {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-900 flex flex-col transition-colors">
      <Navbar />
      <div className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <h1 className="text-3xl font-bold text-slate-900 dark:text-white mb-6">About Us</h1>
        <div className="prose dark:prose-invert max-w-none">
          <p className="text-lg text-slate-600 dark:text-slate-300 mb-4">
            Welcome to eMechanic, your trusted partner in automotive care. We are dedicated to connecting car owners with the best, most reliable workshops in their area.
          </p>
          <p className="text-slate-600 dark:text-slate-300 mb-4">
            Our mission is to bring transparency, efficiency, and trust to the car repair industry. Whether you need a routine oil change or a complex engine repair, eMechanic makes it easy to find, compare, and book services.
          </p>
          <h2 className="text-2xl font-bold text-slate-900 dark:text-white mt-8 mb-4">Our Story</h2>
          <p className="text-slate-600 dark:text-slate-300 mb-4">
            Founded in 2025, eMechanic started with a simple idea: car repair shouldn't be a hassle. We saw a need for a platform that empowers both car owners and workshop owners, creating a seamless experience for everyone involved.
          </p>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default AboutPage;
