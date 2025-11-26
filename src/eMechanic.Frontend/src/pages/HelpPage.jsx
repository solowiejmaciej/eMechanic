import React from 'react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';
import { Search, HelpCircle, FileText, MessageCircle } from 'lucide-react';

const HelpPage = () => {
  return (
    <div className="min-h-screen bg-white dark:bg-slate-900 flex flex-col transition-colors">
      <Navbar />
      <div className="flex-grow pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto w-full">
        <div className="text-center mb-12">
          <h1 className="text-3xl font-bold text-slate-900 dark:text-white mb-4">How can we help you?</h1>
          <div className="max-w-2xl mx-auto relative">
            <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
            <input 
              type="text" 
              placeholder="Search for help articles..." 
              className="w-full pl-12 pr-4 py-3 rounded-xl border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-primary outline-none transition-all"
            />
          </div>
        </div>

        <div className="grid md:grid-cols-3 gap-8 mb-16">
          <div className="p-6 rounded-2xl border border-slate-100 dark:border-slate-700 bg-white dark:bg-slate-800 hover:shadow-lg transition-all cursor-pointer">
            <div className="w-12 h-12 rounded-xl bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 flex items-center justify-center mb-4">
              <HelpCircle className="w-6 h-6" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">Getting Started</h3>
            <p className="text-slate-600 dark:text-slate-400">Learn the basics of setting up your account and booking your first service.</p>
          </div>
          <div className="p-6 rounded-2xl border border-slate-100 dark:border-slate-700 bg-white dark:bg-slate-800 hover:shadow-lg transition-all cursor-pointer">
            <div className="w-12 h-12 rounded-xl bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400 flex items-center justify-center mb-4">
              <FileText className="w-6 h-6" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">Billing & Payments</h3>
            <p className="text-slate-600 dark:text-slate-400">Everything you need to know about payments, invoices, and refunds.</p>
          </div>
          <div className="p-6 rounded-2xl border border-slate-100 dark:border-slate-700 bg-white dark:bg-slate-800 hover:shadow-lg transition-all cursor-pointer">
            <div className="w-12 h-12 rounded-xl bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400 flex items-center justify-center mb-4">
              <MessageCircle className="w-6 h-6" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">Account Support</h3>
            <p className="text-slate-600 dark:text-slate-400">Manage your profile, security settings, and notification preferences.</p>
          </div>
        </div>

        <h2 className="text-2xl font-bold text-slate-900 dark:text-white mb-6">Frequently Asked Questions</h2>
        <div className="space-y-4 max-w-3xl mx-auto">
          {[
            "How do I book a service?",
            "Can I cancel my appointment?",
            "How do I contact the workshop?",
            "Is my payment information secure?"
          ].map((q, i) => (
            <div key={i} className="p-4 rounded-xl border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800 cursor-pointer transition-colors flex justify-between items-center">
              <span className="font-medium text-slate-900 dark:text-white">{q}</span>
              <HelpCircle className="w-5 h-5 text-slate-400" />
            </div>
          ))}
        </div>
      </div>
      <Footer />
    </div>
  );
};

export default HelpPage;
