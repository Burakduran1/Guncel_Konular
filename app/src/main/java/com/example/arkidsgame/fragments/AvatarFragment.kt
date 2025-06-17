package com.example.arkidsgame.fragments

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.ImageView
import androidx.fragment.app.Fragment
import com.example.arkidsgame.CharacterCreationActivity
import com.example.arkidsgame.R

class AvatarFragment : Fragment() {

    private lateinit var avatarImageView: ImageView
    private lateinit var hairStyleButtons: List<ImageButton>
    private lateinit var eyeColorButtons: List<ImageButton>
    private lateinit var clothesButtons: List<ImageButton>
    private lateinit var accessoryButtons: List<ImageButton>
    
    // Seçilen özellikler
    private var currentHairStyle = 0
    private var currentEyeColor = 0
    private var currentClothes = 0
    private var currentAccessory = 0
    
    // Özellik resimleri
    private val hairStyles = listOf(
        R.drawable.avatar_hair_1, 
        R.drawable.avatar_hair_2, 
        R.drawable.avatar_hair_3
    )
    private val eyeColors = listOf(
        R.drawable.avatar_eye_1, 
        R.drawable.avatar_eye_2, 
        R.drawable.avatar_eye_3
    )
    private val clothes = listOf(
        R.drawable.avatar_clothes_1, 
        R.drawable.avatar_clothes_2, 
        R.drawable.avatar_clothes_3
    )
    private val accessories = listOf(
        R.drawable.avatar_accessory_1, 
        R.drawable.avatar_accessory_2, 
        R.drawable.avatar_accessory_3
    )
    
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        val view = inflater.inflate(R.layout.fragment_avatar, container, false)
        
        // UI elemanlarını tanımla
        avatarImageView = view.findViewById(R.id.avatarImageView)
        
        // Saç stili butonları
        hairStyleButtons = listOf(
            view.findViewById(R.id.hairStyle1Button),
            view.findViewById(R.id.hairStyle2Button),
            view.findViewById(R.id.hairStyle3Button)
        )
        
        // Göz rengi butonları
        eyeColorButtons = listOf(
            view.findViewById(R.id.eyeColor1Button),
            view.findViewById(R.id.eyeColor2Button),
            view.findViewById(R.id.eyeColor3Button)
        )
        
        // Kıyafet butonları
        clothesButtons = listOf(
            view.findViewById(R.id.clothes1Button),
            view.findViewById(R.id.clothes2Button),
            view.findViewById(R.id.clothes3Button)
        )
        
        // Aksesuar butonları
        accessoryButtons = listOf(
            view.findViewById(R.id.accessory1Button),
            view.findViewById(R.id.accessory2Button),
            view.findViewById(R.id.accessory3Button)
        )
        
        // Butonların görsellerini ayarla
        setupButtonImages()
        
        // Buton tıklama işlevlerini ayarla
        setupButtonListeners()
        
        // Varsayılan avatarı göster
        updateAvatar()
        
        return view
    }
    
    private fun setupButtonImages() {
        // Saç stili butonları
        hairStyleButtons.forEachIndexed { index, button ->
            button.setImageResource(hairStyles[index])
        }
        
        // Göz rengi butonları
        eyeColorButtons.forEachIndexed { index, button ->
            button.setImageResource(eyeColors[index])
        }
        
        // Kıyafet butonları
        clothesButtons.forEachIndexed { index, button ->
            button.setImageResource(clothes[index])
        }
        
        // Aksesuar butonları
        accessoryButtons.forEachIndexed { index, button ->
            button.setImageResource(accessories[index])
        }
    }
    
    private fun setupButtonListeners() {
        // Saç stili butonları
        hairStyleButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentHairStyle = index
                updateAvatar()
                (activity as CharacterCreationActivity).updateHairStyle(index)
            }
        }
        
        // Göz rengi butonları
        eyeColorButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentEyeColor = index
                updateAvatar()
                (activity as CharacterCreationActivity).updateEyeColor(index)
            }
        }
        
        // Kıyafet butonları
        clothesButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentClothes = index
                updateAvatar()
                (activity as CharacterCreationActivity).updateClothes(index)
            }
        }
        
        // Aksesuar butonları
        accessoryButtons.forEachIndexed { index, button ->
            button.setOnClickListener {
                currentAccessory = index
                updateAvatar()
                (activity as CharacterCreationActivity).updateAccessory(index)
            }
        }
    }
    
    private fun updateAvatar() {
        // Avatar bileşenlerini göster
        // Gerçek bir uygulamada, burada birden fazla ImageView veya bir avatar oluşturma sistemi kullanılabilir
        avatarImageView.setImageResource(clothes[currentClothes])
        
        // Seçili butonları vurgula
        hairStyleButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentHairStyle) 1.0f else 0.5f
        }
        
        eyeColorButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentEyeColor) 1.0f else 0.5f
        }
        
        clothesButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentClothes) 1.0f else 0.5f
        }
        
        accessoryButtons.forEachIndexed { index, button ->
            button.alpha = if (index == currentAccessory) 1.0f else 0.5f
        }
    }
} 