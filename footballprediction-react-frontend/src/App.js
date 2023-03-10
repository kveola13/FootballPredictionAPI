import './App.css';
import DisplayTeams from './DisplayTeams';
import Footer from './Footer';
import Header from './Header';

function App() {
  return (
    <div className="content">
      <Header />
      <DisplayTeams/>
      <Footer />
    </div>
  );
}

export default App;
