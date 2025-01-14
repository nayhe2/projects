import React from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App'
import { createClient } from '@supabase/supabase-js'
import { SessionContextProvider } from '@supabase/auth-helpers-react'

const supabase = createClient(
  "https://ijvdgjmdtkoutcsywthu.supabase.co",
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImlqdmRnam1kdGtvdXRjc3l3dGh1Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzMyNjA3NzgsImV4cCI6MjA0ODgzNjc3OH0.YAx7dkOntbXM1n8wfh2LchmvqnslgDShS2heKFyCSWk"
);

createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <SessionContextProvider supabaseClient={supabase}>
      <App />
    </SessionContextProvider>
  </React.StrictMode>,
)
