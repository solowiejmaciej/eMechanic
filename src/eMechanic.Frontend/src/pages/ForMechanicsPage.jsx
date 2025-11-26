import React from 'react';
import { CheckCircle2, BarChart3, Users, Zap } from 'lucide-react';
import Navbar from '../components/layout/Navbar';
import Footer from '../components/layout/Footer';
import { motion } from 'framer-motion';

const ForMechanicsPage = () => {
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors">
      <Navbar />
      
      {/* Hero */}
      <section className="pt-32 pb-20 px-4 sm:px-6 lg:px-8 bg-slate-900 dark:bg-black text-white relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-slate-900 to-slate-800 dark:from-black dark:to-slate-900 z-0" />
        <div className="max-w-7xl mx-auto relative z-10 text-center">
          <motion.h1 
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="text-4xl lg:text-6xl font-bold mb-6 text-white"
          >
            Grow Your Workshop Business
          </motion.h1>
          <motion.p 
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
            className="text-xl text-slate-300 max-w-2xl mx-auto mb-10"
          >
            Join thousands of workshops using eMechanic to streamline operations, attract new customers, and increase revenue.
          </motion.p>
          <motion.button
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
            className="px-8 py-4 bg-primary hover:bg-primary-hover text-white font-bold rounded-full text-lg shadow-lg shadow-primary/25 transition-all"
          >
            Register Your Workshop
          </motion.button>
        </div>
      </section>

      {/* Benefits */}
      <section className="py-20 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto">
        <div className="grid md:grid-cols-3 gap-8">
          <div className="p-8 bg-white dark:bg-slate-800 rounded-2xl border border-slate-100 dark:border-slate-700 shadow-sm">
            <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-xl flex items-center justify-center mb-6">
              <Users className="w-6 h-6 text-primary" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-3">More Customers</h3>
            <p className="text-slate-600 dark:text-slate-400">
              Get discovered by local car owners actively looking for repair services.
            </p>
          </div>
          <div className="p-8 bg-white dark:bg-slate-800 rounded-2xl border border-slate-100 dark:border-slate-700 shadow-sm">
            <div className="w-12 h-12 bg-orange-100 dark:bg-orange-900/30 rounded-xl flex items-center justify-center mb-6">
              <Zap className="w-6 h-6 text-orange-500" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-3">Streamlined Booking</h3>
            <p className="text-slate-600 dark:text-slate-400">
              Manage appointments and quotes digitally, reducing phone calls and admin work.
            </p>
          </div>
          <div className="p-8 bg-white dark:bg-slate-800 rounded-2xl border border-slate-100 dark:border-slate-700 shadow-sm">
            <div className="w-12 h-12 bg-green-100 dark:bg-green-900/30 rounded-xl flex items-center justify-center mb-6">
              <BarChart3 className="w-6 h-6 text-green-600" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-3">Business Insights</h3>
            <p className="text-slate-600 dark:text-slate-400">
              Track performance, revenue, and customer feedback with detailed analytics.
            </p>
          </div>
        </div>
      </section>
      <Footer />
    </div>
  );
};

export default ForMechanicsPage;
