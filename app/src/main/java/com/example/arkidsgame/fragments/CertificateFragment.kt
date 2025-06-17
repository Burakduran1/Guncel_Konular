package com.example.arkidsgame.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.ImageView
import android.widget.TextView
import androidx.fragment.app.Fragment
import com.example.arkidsgame.CharacterCreationActivity
import com.example.arkidsgame.R

class CertificateFragment : Fragment() {

    private lateinit var certificateImageView: ImageView
    private lateinit var nameEditText: EditText
    private lateinit var completeButton: Button
    private lateinit var oathTextView: TextView
    
    private val oath = "Ben, ____________, bir Dünya Kaşifi olarak, doğayı korumayı, " +
            "yeni yerler keşfetmeyi, diğer canlılara saygı duymayı ve öğrendiklerimi " +
            "başkalarıyla paylaşmayı taahhüt ediyorum."
    
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_certificate, container, false)
        
        // UI elemanlarını tanımla
        certificateImageView = view.findViewById(R.id.certificateImageView)
        nameEditText = view.findViewById(R.id.nameEditText)
        completeButton = view.findViewById(R.id.completeButton)
        oathTextView = view.findViewById(R.id.oathTextView)
        
        // Sertifika görselini ayarla
        certificateImageView.setImageResource(R.drawable.certificate_template)
        
        // Kaşif yeminini göster
        oathTextView.text = oath
        
        // Tamamla butonuna tıklama işlevini ekle
        setupButtonListener()
        
        return view
    }
    
    private fun setupButtonListener() {
        completeButton.setOnClickListener {
            val explorerName = nameEditText.text.toString()
            
            if (explorerName.isNotEmpty()) {
                // Sertifikaya ismi ekle
                val personalizedOath = oath.replace("____________", explorerName)
                oathTextView.text = personalizedOath
                
                // CharacterCreationActivity'ye bilgi ver
                (activity as CharacterCreationActivity).updateExplorerName(explorerName)
                (activity as CharacterCreationActivity).completeCertificate()
            } else {
                // İsim alanı boş, kullanıcıyı uyar
                nameEditText.error = "Lütfen adını gir!"
            }
        }
    }
} 