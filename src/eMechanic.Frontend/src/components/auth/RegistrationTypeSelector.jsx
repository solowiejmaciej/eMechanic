import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { User, Wrench } from 'lucide-react';

const RegistrationTypeSelector = () => {
  const location = useLocation();
  const isWorkshop = location.pathname === '/register-workshop';

  return (
    <div className="flex p-1 mb-8 bg-slate-100 dark:bg-slate-800 rounded-xl">
      <Link
        to="/register"
        className={`flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium rounded-lg transition-all ${
          !isWorkshop
            ? 'bg-white dark:bg-dark-card text-primary shadow-sm'
            : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
        }`}
      >
        <User className="w-4 h-4" />
        Car Owner
      </Link>
      <Link
        to="/register-workshop"
        className={`flex-1 flex items-center justify-center gap-2 py-3 text-sm font-medium rounded-lg transition-all ${
          isWorkshop
            ? 'bg-white dark:bg-dark-card text-secondary shadow-sm'
            : 'text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-200'
        }`}
      >
        <Wrench className="w-4 h-4" />
        Workshop
      </Link>
    </div>
  );
};

export default RegistrationTypeSelector;
