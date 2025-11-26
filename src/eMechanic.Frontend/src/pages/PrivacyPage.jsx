import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';

const PrivacyPage = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors flex flex-col">
      <Navbar />
      <main className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-4xl mx-auto w-full">
        <div className="bg-white dark:bg-dark-card rounded-2xl p-8 md:p-12 shadow-sm border border-slate-100 dark:border-slate-700">
          <h1 className="text-3xl md:text-4xl font-bold text-slate-900 dark:text-white mb-8">Privacy Policy</h1>
          
          <div className="prose prose-slate dark:prose-invert max-w-none">
            <p className="text-lg text-slate-600 dark:text-slate-300 mb-6">
              Last updated: {new Date().toLocaleDateString()}
            </p>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">1. Information We Collect</h2>
              <p className="text-slate-600 dark:text-slate-300 mb-4">
                We collect information you provide directly to us when you create an account, update your profile, or communicate with us. This may include:
              </p>
              <ul className="list-disc pl-6 space-y-2 text-slate-600 dark:text-slate-300">
                <li>Name and contact information</li>
                <li>Vehicle information</li>
                <li>Service history and preferences</li>
                <li>Payment information</li>
              </ul>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">2. How We Use Your Information</h2>
              <p className="text-slate-600 dark:text-slate-300 mb-4">
                We use the information we collect to:
              </p>
              <ul className="list-disc pl-6 space-y-2 text-slate-600 dark:text-slate-300">
                <li>Provide, maintain, and improve our services</li>
                <li>Process transactions and send related information</li>
                <li>Send you technical notices, updates, and support messages</li>
                <li>Connect you with workshops and facilitate appointments</li>
              </ul>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">3. Information Sharing</h2>
              <p className="text-slate-600 dark:text-slate-300">
                We share your information with workshops when you book a service. We do not sell your personal information to third parties. We may share generic aggregated demographic information not linked to any personal identification information.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">4. Data Security</h2>
              <p className="text-slate-600 dark:text-slate-300">
                We implement appropriate data collection, storage, and processing practices and security measures to protect against unauthorized access, alteration, disclosure, or destruction of your personal information.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">5. Your Rights</h2>
              <p className="text-slate-600 dark:text-slate-300">
                You have the right to access, correct, or delete your personal information. You can manage your information through your account settings or by contacting us.
              </p>
            </section>

            <section>
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">6. Contact Us</h2>
              <p className="text-slate-600 dark:text-slate-300">
                If you have any questions about this Privacy Policy, please contact us at privacy@emechanic.com.
              </p>
            </section>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default PrivacyPage;
