import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Mail, Lock, ArrowRight, Wrench, User } from 'lucide-react';
import Navbar from '../../components/layout/Navbar';

const LoginPage = () => {
  const [activeTab, setActiveTab] = useState('user'); // 'user' or 'workshop'

  return (
    <div className="min-h-screen bg-slate-50 dark:bg-dark-lighter transition-colors">
      <Navbar />
      <div className="pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-md mx-auto">
        <div className="bg-white dark:bg-dark-card rounded-2xl shadow-xl border border-slate-100 dark:border-slate-700 p-8">
          <div className="text-center mb-8">
            <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-2">Welcome Back</h2>
            <p className="text-slate-600 dark:text-slate-400">Sign in to your account</p>
          </div>

          {/* Tabs */}
          <div className="flex p-1 bg-slate-100 dark:bg-slate-800 rounded-xl mb-8">
            <button
              onClick={() => setActiveTab('user')}
              className={`flex-1 flex items-center justify-center gap-2 py-2.5 text-sm font-medium rounded-lg transition-all ${
                activeTab === 'user'
                  ? 'bg-white dark:bg-dark shadow-sm text-primary'
                  : 'text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-200'
              }`}
            >
              <User className="w-4 h-4" />
              Car Owner
            </button>
            <button
              onClick={() => setActiveTab('workshop')}
              className={`flex-1 flex items-center justify-center gap-2 py-2.5 text-sm font-medium rounded-lg transition-all ${
                activeTab === 'workshop'
                  ? 'bg-white dark:bg-dark shadow-sm text-secondary'
                  : 'text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-200'
              }`}
            >
              <Wrench className="w-4 h-4" />
              Workshop
            </button>
          </div>

          <form className="space-y-6">
            <div>
              <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Email Address
              </label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input
                  type="email"
                  className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent outline-none transition-all dark:text-white"
                  placeholder="name@example.com"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Password
              </label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                <input
                  type="password"
                  className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-primary focus:border-transparent outline-none transition-all dark:text-white"
                  placeholder="••••••••"
                />
              </div>
            </div>

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" className="rounded border-slate-300 text-primary focus:ring-primary" />
                <span className="text-slate-600 dark:text-slate-400">Remember me</span>
              </label>
              <a href="#" className="text-primary hover:text-primary-hover font-medium">
                Forgot password?
              </a>
            </div>

            <button
              type="submit"
              className={`w-full py-3.5 px-4 rounded-xl text-white font-medium shadow-lg transition-all flex items-center justify-center gap-2 group ${
                activeTab === 'user'
                  ? 'bg-primary hover:bg-primary-hover shadow-primary/25 hover:shadow-primary/40'
                  : 'bg-secondary hover:bg-secondary-hover shadow-secondary/25 hover:shadow-secondary/40'
              }`}
            >
              Sign In
              <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
            </button>
          </form>

          {activeTab === 'user' && (
            <>
              <div className="relative my-8">
                <div className="absolute inset-0 flex items-center">
                  <div className="w-full border-t border-slate-200 dark:border-slate-700"></div>
                </div>
                <div className="relative flex justify-center text-sm">
                  <span className="px-2 bg-white dark:bg-dark-card text-slate-500">Or continue with</span>
                </div>
              </div>

              <button className="w-full py-3 px-4 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl text-slate-700 dark:text-slate-200 font-medium hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors flex items-center justify-center gap-3">
                <img src="https://www.svgrepo.com/show/475656/google-color.svg" alt="Google" className="w-5 h-5" />
                Sign in with Google
              </button>
            </>
          )}

          <p className="mt-8 text-center text-sm text-slate-600 dark:text-slate-400">
            Don't have an account?{' '}
            <Link
              to={activeTab === 'user' ? '/register' : '/register-workshop'}
              className={`font-medium ${
                activeTab === 'user' ? 'text-primary hover:text-primary-hover' : 'text-secondary hover:text-secondary-hover'
              }`}
            >
              Sign up as {activeTab === 'user' ? 'Car Owner' : 'Workshop'}
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
