import React from 'react';
import { motion } from 'framer-motion';
import { ArrowRight, Search, Shield, Star, Car } from 'lucide-react';
import { Link } from 'react-router-dom';

const HeroSection = () => {
  return (
    <section className="relative pt-32 pb-20 lg:pt-48 lg:pb-32 overflow-hidden transition-colors">
      {/* Background Elements */}
      <div className="absolute top-0 left-1/2 -translate-x-1/2 w-full h-full -z-10">
        <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-primary/20 rounded-full blur-3xl opacity-50" />
        <div className="absolute bottom-0 left-0 w-[500px] h-[500px] bg-secondary/20 rounded-full blur-3xl opacity-50" />
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid lg:grid-cols-2 gap-12 lg:gap-8 items-center">
          {/* Text Content */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6 }}
          >
            <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-white/50 dark:bg-white/10 backdrop-blur-sm border border-white/20 text-primary font-medium text-sm mb-6 shadow-sm">
              <span className="relative flex h-2 w-2">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary opacity-75"></span>
                <span className="relative inline-flex rounded-full h-2 w-2 bg-primary"></span>
              </span>
              #1 Trusted Mechanic Network
            </div>
            
            <h1 className="text-5xl lg:text-7xl font-bold text-slate-900 dark:text-white leading-tight mb-6 dark:drop-shadow-lg">
              Car Repair <br />
              <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary to-blue-400 dark:drop-shadow-none">
                Made Simple.
              </span>
            </h1>
            
            <p className="text-lg text-slate-600 dark:text-slate-200 mb-8 max-w-lg leading-relaxed dark:drop-shadow-md font-medium">
              Connect with top-rated local mechanics instantly. Compare quotes, book appointments, and track repairs - all in one place.
            </p>

            <div className="flex flex-col sm:flex-row gap-4">
              <Link 
                to="/find-workshop"
                className="inline-flex items-center justify-center px-8 py-4 text-lg font-medium text-white bg-primary rounded-full hover:bg-primary-hover transition-all shadow-lg shadow-primary/25 hover:shadow-primary/40 group"
              >
                Find a Workshop
                <ArrowRight className="ml-2 w-5 h-5 group-hover:translate-x-1 transition-transform" />
              </Link>
            </div>

            <div className="mt-12 flex items-center gap-8 text-slate-500 dark:text-slate-400">
              <div className="flex items-center gap-2">
                <Shield className="w-5 h-5 text-primary" />
                <span className="text-sm font-medium">Verified Pros</span>
              </div>
              <div className="flex items-center gap-2">
                <Star className="w-5 h-5 text-secondary" />
                <span className="text-sm font-medium">4.9/5 Rating</span>
              </div>
              <div className="flex items-center gap-2">
                <Search className="w-5 h-5 text-primary" />
                <span className="text-sm font-medium">Instant Quotes</span>
              </div>
            </div>
          </motion.div>

          {/* Visual Content */}
          <motion.div
            initial={{ opacity: 0, x: 20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.8, delay: 0.2 }}
            className="relative"
          >
            <div className="relative z-10 bg-white dark:bg-dark-card rounded-2xl shadow-2xl border border-slate-100 dark:border-slate-700 p-4 rotate-2 hover:rotate-0 transition-transform duration-500">
              {/* Mockup Header */}
              <div className="flex items-center gap-4 mb-6 border-b border-slate-50 dark:border-slate-700 pb-4">
                <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center">
                  <Car className="w-6 h-6 text-primary" />
                </div>
                <div>
                  <h3 className="font-bold text-slate-900 dark:text-white">Vehicle Status</h3>
                  <p className="text-sm text-slate-500 dark:text-slate-400">BMW 3 Series • 2021</p>
                </div>
                <div className="ml-auto px-3 py-1 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 text-xs font-bold rounded-full">
                  Active
                </div>
              </div>

              {/* Mockup Content */}
              <div className="space-y-4">
                <div className="p-4 bg-slate-50 dark:bg-slate-800 rounded-xl border border-slate-100 dark:border-slate-700">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-slate-700 dark:text-slate-300">Oil Change</span>
                    <span className="text-sm font-bold text-slate-900 dark:text-white">$89.00</span>
                  </div>
                  <div className="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                    <div className="bg-primary h-2 rounded-full w-3/4"></div>
                  </div>
                </div>
                
                <div className="p-4 bg-slate-50 dark:bg-slate-800 rounded-xl border border-slate-100 dark:border-slate-700">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm font-medium text-slate-700 dark:text-slate-300">Brake Inspection</span>
                    <span className="text-sm font-bold text-slate-900 dark:text-white">Pending</span>
                  </div>
                  <div className="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                    <div className="bg-secondary h-2 rounded-full w-1/2"></div>
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-xl text-center">
                    <div className="text-2xl font-bold text-primary mb-1">24h</div>
                    <div className="text-xs text-slate-600 dark:text-slate-400">Avg. Turnaround</div>
                  </div>
                  <div className="p-4 bg-orange-50 dark:bg-orange-900/20 rounded-xl text-center">
                    <div className="text-2xl font-bold text-secondary mb-1">15+</div>
                    <div className="text-xs text-slate-600 dark:text-slate-400">Local Shops</div>
                  </div>
                </div>
              </div>
            </div>

            {/* Decorative Elements */}
            <div className="absolute -top-10 -right-10 w-32 h-32 bg-secondary/20 rounded-full blur-2xl -z-10" />
            <div className="absolute -bottom-10 -left-10 w-32 h-32 bg-primary/20 rounded-full blur-2xl -z-10" />
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
