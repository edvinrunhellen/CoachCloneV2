import React, { useState } from 'react';


const JournalUpload: React.FC = () => {
  const [text, setText] = useState('');
  const [token, setToken] = useState('');

  const handleUpload = async () => {
  try {
    const response = await fetch('http://localhost:5144/api/journal/upload', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ content: text }),
    });

    if (response.ok) {
      alert('Journal uploaded!');
    } else {
      alert('Error uploading journal');
    }
  } catch (error) {
    alert('Error uploading journal');
    console.error(error);
  }
};


  return (
    <div>
      <h2>Upload Journal</h2>
      <textarea
        rows={10}
        cols={50}
        value={text}
        onChange={(e) => setText(e.target.value)}
      />
      <br />
      <input
        type="text"
        placeholder="Paste your JWT token here"
        value={token}
        onChange={(e) => setToken(e.target.value)}
        style={{ width: '400px' }}
      />
      <br />
      <button onClick={handleUpload}>Upload</button>
    </div>
  );
};

export default JournalUpload;
