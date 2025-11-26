import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';

const CookiesPage = () => {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-900 flex flex-col transition-colors">
      <Navbar />
      <div className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <h1 className="text-3xl font-bold text-slate-900 dark:text-white mb-6">Cookie Policy</h1>
        <div className="prose dark:prose-invert max-w-none">
          <p className="text-lg text-slate-600 dark:text-slate-300 mb-4">
            This Cookie Policy explains how eMechanic uses cookies and similar technologies to recognize you when you visit our website.
          </p>
          <h2 className="text-2xl font-bold text-slate-900 dark:text-white mt-8 mb-4">What are cookies?</h2>
          <p className="text-slate-600 dark:text-slate-300 mb-4">
            Cookies are small data files that are placed on your computer or mobile device when you visit a website. Cookies are widely used by website owners in order to make their websites work, or to work more efficiently, as well as to provide reporting information.
          </p>
          <h2 className="text-2xl font-bold text-slate-900 dark:text-white mt-8 mb-4">How we use cookies</h2>
          <p className="text-slate-600 dark:text-slate-300 mb-4">
            We use cookies for several reasons. Some cookies are required for technical reasons in order for our website to operate, and we refer to these as "essential" or "strictly necessary" cookies. Other cookies also enable us to track and target the interests of our users to enhance the experience on our Online Properties.
          </p>
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default CookiesPage;
