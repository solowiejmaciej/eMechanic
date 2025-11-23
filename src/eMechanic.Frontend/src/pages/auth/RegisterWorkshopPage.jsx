import React from 'react';
import { Link } from 'react-router-dom';
import { Mail, Lock, User, ArrowRight, MapPin, Phone, Building2 } from 'lucide-react';
import Navbar from '../../components/layout/Navbar';
import RegistrationTypeSelector from '../../components/auth/RegistrationTypeSelector';

const RegisterWorkshopPage = () => {
  return (
    <div className="min-h-screen bg-slate-50 dark:bg-dark-lighter transition-colors">
      <Navbar />
      <div className="pt-32 pb-12 px-4 sm:px-6 lg:px-8 max-w-2xl mx-auto">
        <div className="bg-white dark:bg-dark-card rounded-2xl shadow-xl border border-slate-100 dark:border-slate-700 p-8">
          <RegistrationTypeSelector />
          <div className="text-center mb-8">
            <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-2">Partner with eMechanic</h2>
            <p className="text-slate-600 dark:text-slate-400">Register your Workshop</p>
          </div>

          <form className="space-y-6">
            {/* Account Info */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-slate-900 dark:text-white border-b border-slate-100 dark:border-slate-700 pb-2">
                Account Information
              </h3>
              <div className="grid md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Login Email
                  </label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <input
                      type="email"
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="login@workshop.com"
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
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="••••••••"
                    />
                  </div>
                </div>
              </div>
            </div>

            {/* Workshop Details */}
            <div className="space-y-4 pt-4">
              <h3 className="text-lg font-semibold text-slate-900 dark:text-white border-b border-slate-100 dark:border-slate-700 pb-2">
                Workshop Details
              </h3>
              
              <div className="grid md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Workshop Name
                  </label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <input
                      type="text"
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="Legal Name"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Display Name
                  </label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <input
                      type="text"
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="Public Name"
                    />
                  </div>
                </div>
              </div>

              <div className="grid md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Contact Email
                  </label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <input
                      type="email"
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="contact@workshop.com"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Phone Number
                  </label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                    <input
                      type="tel"
                      className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                      placeholder="+1 234 567 890"
                    />
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Address
                </label>
                <div className="relative">
                  <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
                  <input
                    type="text"
                    className="w-full pl-10 pr-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                    placeholder="123 Main St"
                  />
                </div>
              </div>

              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    City
                  </label>
                  <input
                    type="text"
                    className="w-full px-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                    placeholder="New York"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Postal Code
                  </label>
                  <input
                    type="text"
                    className="w-full px-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                    placeholder="10001"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Country
                  </label>
                  <input
                    type="text"
                    className="w-full px-4 py-3 bg-slate-50 dark:bg-slate-800 border border-slate-200 dark:border-slate-700 rounded-xl focus:ring-2 focus:ring-secondary focus:border-transparent outline-none transition-all dark:text-white"
                    placeholder="USA"
                  />
                </div>
              </div>
            </div>

            <button
              type="submit"
              className="w-full py-3.5 px-4 bg-secondary hover:bg-secondary-hover text-white font-medium rounded-xl shadow-lg shadow-secondary/25 hover:shadow-secondary/40 transition-all flex items-center justify-center gap-2 group"
            >
              Register Workshop
              <ArrowRight className="w-5 h-5 group-hover:translate-x-1 transition-transform" />
            </button>
          </form>

          <p className="mt-8 text-center text-sm text-slate-600 dark:text-slate-400">
            Already have an account?{' '}
            <Link to="/login" className="text-secondary hover:text-secondary-hover font-medium">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default RegisterWorkshopPage;
