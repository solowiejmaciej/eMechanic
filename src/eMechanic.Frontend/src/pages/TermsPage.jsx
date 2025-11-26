import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';

const TermsPage = () => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-secondary/10 via-slate-50 to-primary/10 dark:bg-slate-900 dark:bg-none transition-colors flex flex-col">
      <Navbar />
      <main className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-4xl mx-auto w-full">
        <div className="bg-white dark:bg-dark-card rounded-2xl p-8 md:p-12 shadow-sm border border-slate-100 dark:border-slate-700">
          <h1 className="text-3xl md:text-4xl font-bold text-slate-900 dark:text-white mb-8">Terms of Service</h1>
          
          <div className="prose prose-slate dark:prose-invert max-w-none">
            <p className="text-lg text-slate-600 dark:text-slate-300 mb-6">
              Last updated: {new Date().toLocaleDateString()}
            </p>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">1. Acceptance of Terms</h2>
              <p className="text-slate-600 dark:text-slate-300">
                By accessing and using eMechanic ("the Service"), you agree to be bound by these Terms of Service. If you do not agree to these terms, please do not use the Service.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">2. User Accounts</h2>
              <p className="text-slate-600 dark:text-slate-300 mb-4">
                When you create an account with us, you must provide information that is accurate, complete, and current at all times. Failure to do so constitutes a breach of the Terms, which may result in immediate termination of your account on our Service.
              </p>
              <p className="text-slate-600 dark:text-slate-300">
                You are responsible for safeguarding the password that you use to access the Service and for any activities or actions under your password.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">3. Services</h2>
              <p className="text-slate-600 dark:text-slate-300">
                eMechanic connects vehicle owners with automotive workshops. We are not responsible for the quality of services provided by workshops. Any agreement or transaction is strictly between the vehicle owner and the workshop.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">4. Intellectual Property</h2>
              <p className="text-slate-600 dark:text-slate-300">
                The Service and its original content, features, and functionality are and will remain the exclusive property of eMechanic and its licensors.
              </p>
            </section>

            <section className="mb-8">
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">5. Termination</h2>
              <p className="text-slate-600 dark:text-slate-300">
                We may terminate or suspend access to our Service immediately, without prior notice or liability, for any reason whatsoever, including without limitation if you breach the Terms.
              </p>
            </section>

            <section>
              <h2 className="text-2xl font-semibold text-slate-900 dark:text-white mb-4">6. Contact Us</h2>
              <p className="text-slate-600 dark:text-slate-300">
                If you have any questions about these Terms, please contact us at support@emechanic.com.
              </p>
            </section>
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
};

export default TermsPage;
