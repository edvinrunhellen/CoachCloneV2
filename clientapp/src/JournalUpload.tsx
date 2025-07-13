import React, { useState } from 'react';


const JournalUpload: React.FC = () => {
    const [file, setFile] = useState<File | null>(null);
    const [token, setToken] = useState('');
    const [message, setMessage] = useState('');

  const handleUpload = async () => {
  if (!file) {
    setMessage('❌ Please select a file.');
    return;
  }

  if (file.size > 5 * 1024 * 1024) {
    setMessage('❌ File too large (max 5 MB).');
    return;
  }

  if (file.type !== 'application/pdf' && file.type !== 'text/plain') {
    setMessage('❌ Only PDF or text files are allowed.');
    return;
  }

  const formData = new FormData();
  formData.append('file', file);

  try {
    const response = await fetch('http://localhost:5144/api/journal/upload', {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${token}`,
      },
      body: formData,
    });

    if (response.ok) {
      setMessage('✅ Upload successful!');
    } else {
      setMessage('❌ Error uploading journal.');
    }
  } catch (error) {
    setMessage('❌ Error uploading journal.');
    console.error(error);
  }
};



  return (
    <div>
     <input
  type="file"
  onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
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

{message && <p>{message}</p>}
    </div>
  );
};

export default JournalUpload;
